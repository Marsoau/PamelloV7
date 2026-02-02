using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Points;
using PamelloV7.Core.Audio.Time;
using PamelloV7.Core.Downloads;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Extensions;

namespace PamelloV7.Audio.Modules;

public class SongAudio : IAudioModuleWithOutput
{
    public List<IAudioPoint> Outputs { get; }
    public IAudioPoint Output => Outputs.First();

    public IPamelloSong Song { get; }

    private int _currentChunkPosition;
    private MemoryStream? _currentChunk;
    private MemoryStream? _nextChunk;

    private AudioTime _position;
    private AudioTime _duration;

    public AudioTime Position
        => _position;
    public AudioTime Duration
        => _duration;

    private int? _nextBreakPoint;
    private int? _nextJumpPoint;

    private Process? _ffmpeg;
    private long _ffmpegPosition;

    private Task? _rewinding;

    public bool IsInitialized {
        get => _currentChunk is not null;
    }

    private AudioTime _chunkSize;

    private Task<MemoryStream?>? _nextChunkTask;

    public int MinOutputs => 1;
    public int MaxOutputs => 1;

    public bool IsEnded { get; private set; }
    public event Action OnEnded;

    public bool IsDisposed { get; private set; }

    public SongAudio(
        IPamelloSong song,
        IServiceProvider services
    ) {
        Song = song;

        Outputs = new List<IAudioPoint>(1);

        _position = new AudioTime(0);
        _duration = new AudioTime(0);

        _chunkSize = new AudioTime(8);
            
        IsEnded = false;
    }

    public void Clean() {
        _currentChunk?.Close();
        _currentChunk?.Dispose();
        _currentChunk = null;
        _nextChunk?.Close();
        _nextChunk?.Dispose();
        _nextChunk = null;

        _ffmpeg?.Close();
        _ffmpeg?.Dispose();
        _ffmpeg = null;

        _ffmpegPosition = 0;

        _nextBreakPoint = null;
        _nextJumpPoint = null;

        _position.TimeValue = 0;
        _duration.TimeValue = 0;
    }

    public async Task<bool> TryInitialize(CancellationToken token = default) {
        if (IsInitialized) return true;

        if (!(Song.SelectedSource?.IsDownloaded() ?? false)) { //if (!Song.SelectedSource.IsDownloaded) {
            if (Song.SelectedSource is null) return false;
            
            if (await Song.SelectedSource.GetDownloader().DownloadAsync() != EDownloadResult.Success) {
                Clean();
                return false;
            }
        }

        _duration.TotalSeconds = GetSongDuration(Song)?.TotalSeconds ?? 0;

        await LoadChunksAtAsync(0, token);
        if (_currentChunk is null) {
            Console.WriteLine("interesting");
            return false;
        }

        _currentChunkPosition = 0;

        //Song.PlayCount++;

        return true;
    }

    private bool NextBytes(byte[] result, bool wait, CancellationToken token) {
        return NextBytesAsync(result, wait, token).Result;
    }
    private async Task<bool> NextBytesAsync(byte[] result, bool wait, CancellationToken token) {
        // Console.WriteLine("next bytes");
        if (_rewinding is not null) await _rewinding;
        if (_currentChunk is null) return await TryInitialize(token);
        // Console.WriteLine("next bytes 2");

        if (_position.TimeValue >= _duration.TimeValue) {
            if (IsEnded) return false;
                
            IsEnded = true;
            OnEnded?.Invoke();
            return false;
        }

        if (_position.TotalSeconds == _nextBreakPoint) {
            if (_nextJumpPoint is null) {
                await RewindTo(_duration, true, token);
                // Console.WriteLine("false 2");
                return false;
            }

            await RewindTo(new AudioTime(_nextJumpPoint.Value), true, token);
        }
            
        if (_position.TimeValue / _chunkSize.TimeValue > _currentChunkPosition) {
            await MoveForwardAsync(token);
        }

        _currentChunk.Position = _position.TimeValue % _chunkSize.TimeValue;
        var count = await _currentChunk.ReadAsync(result, token);
        //Console.WriteLine($"{count}, {result.Length}, {_currentChunk.Length}");
        if (count != result.Length) return false;
            
        _position.TimeValue += result.Length;
        return true;
    }

    public void UpdatePlaybackPoints(bool excludeCurrent = false) {
        int? closestBreakPoint = null;
        int? closestJumpPoint = null;

        foreach (var episode in Song.Episodes) {
            if (
                (excludeCurrent) ?
                    (episode.Start.TotalSeconds > _position.TotalSeconds) :
                    (episode.Start.TotalSeconds >= _position.TotalSeconds)
            ) {
                if (episode.AutoSkip) {
                    if (closestBreakPoint is null || episode.Start.TotalSeconds < closestBreakPoint) {
                        closestBreakPoint = episode.Start.TotalSeconds;
                    }
                }
                else if (episode.Start.TotalSeconds > closestBreakPoint) {
                    if (closestJumpPoint is null || episode.Start.TotalSeconds < closestJumpPoint) {
                        closestJumpPoint = episode.Start.TotalSeconds;
                    }
                }
            }
        }

        _nextBreakPoint = closestBreakPoint;
        _nextJumpPoint = closestJumpPoint;

        //Console.WriteLine($"Updated playback points: [{_nextBreakPoint}] | [{_nextJumpPoint}]");
    }

    public async Task RewindTo(AudioTime time, bool forceEpisodePlayback, CancellationToken token) {
        var rewindCompletion = new TaskCompletionSource();
        while (_rewinding is not null) {
            await _rewinding;
        }

        _rewinding = rewindCompletion.Task;

        if (_currentChunk is null) return;

        long timePosition = time.TimeValue / _chunkSize.TimeValue;
        long timeDifferece = time.TimeValue % _chunkSize.TimeValue;

        if (timePosition == _currentChunkPosition + 1) {
            await MoveForwardAsync(token);
        }
        else if (timePosition != _currentChunkPosition) {
            await LoadChunksAtAsync((int)timePosition, token);
        }

        _position.TimeValue = time.TimeValue;

        UpdatePlaybackPoints(forceEpisodePlayback);

        rewindCompletion.SetResult();
        _rewinding = null;
    }
    public async Task<IPamelloEpisode?> RewindToEpisode(int episodePosition, bool forceEpisodePlayback = true) {
        var episode = Song.Episodes.ElementAtOrDefault(episodePosition);
        if (episode is null) {
            if (episodePosition > 0) {
                await RewindTo(_duration, forceEpisodePlayback, CancellationToken.None);
            }
            else {
                await RewindTo(new AudioTime(0), forceEpisodePlayback, CancellationToken.None);
            }

            return null;
        }

        await RewindTo(episode.Start, forceEpisodePlayback, CancellationToken.None);
        return episode;
    }

    public int? GetCurrentEpisodePosition() {
        int? closestLeft = null;

        for (int i = 0; i < Song.Episodes.Count; i++) {
            if (Song.Episodes[i].Start.TotalSeconds <= _position.TotalSeconds &&
                Song.Episodes[i].Start.TotalSeconds > 
                (closestLeft is not null ?
                    Song.Episodes[closestLeft.Value].Start.TotalSeconds :
                    int.MinValue
                )
               ) {
                closestLeft = i;
            }
        }

        return closestLeft;
    }
    public IPamelloEpisode? GetCurrentEpisode() {
        var currentPosition = GetCurrentEpisodePosition();
        if (currentPosition is null) return null;

        return Song.Episodes.ElementAtOrDefault(currentPosition.Value);
    }

    public async Task LoadChunksAtAsync(int position, CancellationToken token) {
        if (_nextChunkTask is not null) await _nextChunkTask;
            
        _currentChunkPosition = position;
        _currentChunk = await LoadChunkAsync(_currentChunkPosition, token);
        _nextChunkTask = LoadChunkAsync(_currentChunkPosition + 1, token);
            
        _ = Task.Run(async () =>
        {
            _nextChunk = await _nextChunkTask;
            _nextChunkTask = null;
        });
    }
    private async Task MoveForwardAsync(CancellationToken token) {
        if (_nextChunkTask is not null) await _nextChunkTask;
        _currentChunk = _nextChunk;
        _nextChunkTask = LoadChunkAsync(++_currentChunkPosition + 1, token);
            
        _ = Task.Run(async () =>
        {
            _nextChunk = await _nextChunkTask;
            _nextChunkTask = null;
        }, token);
    }

    private void CreateFFMpeg(AudioTime start = default) {
        if (_ffmpeg is not null) {
            _ffmpeg.Dispose();
            _ffmpeg = null;
            _ffmpegPosition = start.TimeValue;
        }
        
        if (Song.SelectedSource is null) throw new PamelloException("Song source is null, cannot create FFMpeg");

        _ffmpeg = Process.Start(new ProcessStartInfo {
            FileName = "ffmpeg",
            //Arguments = $@"-speed ultrafast -hide_banner -loglevel panic -i ""{GetSongAudioPath(Song)}"" -ac 2 -f s16le -ar 48000 pipe:1",
            Arguments = $@"-hide_banner -loglevel panic -ss {start} -i ""{Song.SelectedSource.GetFile().FullName}"" -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        });
    }

    private async Task<MemoryStream?> LoadChunkAsync(int position, CancellationToken token) {
        return await Task.Run(() => LoadChunk(position, token));
    }
        
    private async Task<MemoryStream?> LoadChunk(int position, CancellationToken token) {
        //Console.WriteLine($"->\n-> loading chunk at {position}\n->");

        if (_ffmpeg is null) {
            //Console.WriteLine("ffmpeg creating 1");
            CreateFFMpeg();
            //Console.WriteLine("ffmpeg created 1");
        }
        else {
            _ffmpeg.Resume();
            //_ffmpeg.Resume();
        }

        if (_ffmpeg is null) return null;
        //Console.WriteLine("ffmpeg is not null");

        long startPos = _chunkSize.TimeValue * position;
        long endPos = _chunkSize.TimeValue * (position + 1);
        //Console.WriteLine($"Next length: {endPos - startPos}");

        if (_ffmpegPosition != startPos) {
            //Console.WriteLine("ffmpeg creating 2");
            CreateFFMpeg(new AudioTime(startPos));
            //Console.WriteLine("ffmpeg created 2");
        }

        while (
            _ffmpegPosition < startPos &&
            !_ffmpeg.StandardOutput.EndOfStream &&
            !token.IsCancellationRequested
        ) {
            _ffmpeg.StandardOutput.BaseStream.ReadByte();
            _ffmpegPosition += 1;
        }

        var chunkStream = new MemoryStream();

        while (_ffmpegPosition < endPos && !token.IsCancellationRequested) {
            if (_ffmpeg.StandardOutput.EndOfStream) {
                if (chunkStream.Length > 0) {
                    //Console.WriteLine($"<-\n<- loaded CUT chunk at {position}\n<-\n");
                    return chunkStream;
                }
                else {
                    //Console.WriteLine($"failed to load chunk stream at {position}");
                    return null;
                }
            }

            chunkStream.WriteByte((byte)_ffmpeg.StandardOutput.BaseStream.ReadByte());
            _ffmpegPosition++;
        }
            
        _ffmpeg.Suspend();

        //Console.WriteLine($"<-\n<- loaded chunk at {position}\n<-\n");
        return chunkStream;
    }

    public static AudioTime? GetSongDuration(IPamelloSong song) {
        if (!(song.SelectedSource?.IsDownloaded() ?? false)) {
            return null;
        }

        using var ffprobeProcess = Process.Start(new ProcessStartInfo {
            FileName = "ffprobe",
            Arguments = $@"-i ""{song.SelectedSource.GetFile().FullName}"" -show_entries format=duration -v quiet -of csv=""p=0""",
            UseShellExecute = false,
            RedirectStandardOutput = true
        });

        var ffprobeStream = ffprobeProcess?.StandardOutput.BaseStream;
        if (ffprobeStream is null) return null;

        var durationStr = new StreamReader(ffprobeStream).ReadToEnd();

        if (int.TryParse(durationStr.Split('.').First(), out var duration)) {
            return new AudioTime(duration);
        }
            
        return null;
    }

    public void InitAudio() {
        Output.ProcessAudio = NextBytes;
    }

    public void Dispose()
    {
        IsDisposed = true;
        Clean();
            
        _currentChunk?.Dispose();
        _nextChunk?.Dispose();
        _ffmpeg?.Dispose();
        _rewinding?.Dispose();
            
        //Output.Dispose();
    }
}
