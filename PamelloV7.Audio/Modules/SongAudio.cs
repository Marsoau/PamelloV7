using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Points;
using PamelloV7.Framework.Dependencies.Service;
using PamelloV7.Framework.Downloads;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Enumerators;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Logging;
using PamelloV7.Server.Extensions;

namespace PamelloV7.Audio.Modules;

public partial class SongAudio : AudioModule, IAudioModuleWithOutput
{
    private readonly IDependenciesService _dependencies;

    public IPamelloSong Song { get; }

    private int _currentChunkPosition;
    private MemoryStream? _currentChunk;
    private MemoryStream? _nextChunk;

    public AudioTime Position;
    public AudioTime Duration;

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

    public bool IsEnded { get; private set; }
    public Action OnEnded;

    public bool IsDisposed { get; private set; }

    public SongAudio(
        IPamelloSong song,
        IServiceProvider services
    ) {
        _dependencies = services.GetRequiredService<IDependenciesService>();
        
        Song = song;

        Position = new AudioTime(0);
        Duration = new AudioTime(0);

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

        Position.TimeValue = 0;
        Duration.TimeValue = 0;
    }

    public async Task<bool> TryInitialize(CancellationToken token = default) {
        if (IsInitialized) return true;

        if (!(Song.SelectedSource?.IsDownloaded() ?? false)) { //if (!Song.SelectedSource.IsDownloaded) {
            if (Song.SelectedSource is null) return false;

            EDownloadResult result;
            try {
                result = await Song.SelectedSource.GetDownloader().DownloadAsync();
            }
            catch (Exception x) {
                StaticLogger.Log($"Exception downloading song {Song}: {x}");
                Clean();
                return false;
            }
            
            if (result != EDownloadResult.Success) {
                Clean();
                return false;
            }
        }

        Duration.TotalSeconds = GetSongDuration(Song, _dependencies)?.TotalSeconds ?? 0;

        await LoadChunksAtAsync(0, token);
        if (_currentChunk is null) {
            StaticLogger.Log("interesting");
            return false;
        }

        _currentChunkPosition = 0;

        //Song.PlayCount++;

        return true;
    }

    private bool NextBytes(byte[] result, bool wait, CancellationToken token) {
        return NextBytesAsync(result, wait, token).Result;
    }
    private async Task<bool> NextBytesAsync(byte[] audio, bool wait, CancellationToken token) {
        if (_rewinding is not null) await _rewinding;
        if (_currentChunk is null) return false; //return await TryInitialize(token);

        if (Position.TimeValue >= Duration.TimeValue) {
            if (IsEnded) return false;
                
            IsEnded = true;
            OnEnded?.Invoke();
            return false;
        }

        if (Position.TotalSeconds == _nextBreakPoint) {
            if (_nextJumpPoint is null) {
                await RewindTo(Duration, true, token);
                // StaticLogger.Log("false 2");
                return false;
            }

            await RewindTo(new AudioTime(_nextJumpPoint.Value), true, token);
        }

        int count;
        var totalCount = 0;
        while (totalCount < audio.Length && Position.TimeValue < Duration.TimeValue) {
            totalCount += count = await _currentChunk.ReadAsync(audio, totalCount, audio.Length - totalCount, token);
            
            Position.TimeValue += count;
            
            if (totalCount >= audio.Length) return true;
            
            await MoveForwardAsync(token);
        }

        return false;
    }

    public void UpdatePlaybackPoints(bool excludeCurrent = false) {
        int? closestBreakPoint = null;
        int? closestJumpPoint = null;

        foreach (var episode in Song.Episodes) {
            if (
                (excludeCurrent) ?
                    (episode.Start.TotalSeconds > Position.TotalSeconds) :
                    (episode.Start.TotalSeconds >= Position.TotalSeconds)
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

        //StaticLogger.Log($"Updated playback points: [{_nextBreakPoint}] | [{_nextJumpPoint}]");
    }

    public async Task RewindTo(AudioTime time, bool forceEpisodePlayback = true, CancellationToken token = default) {
        var rewindCompletion = new TaskCompletionSource();
        while (_rewinding is not null) {
            await _rewinding;
        }

        _rewinding = rewindCompletion.Task;

        if (_currentChunk is null) return;

        long timePosition = time.TimeValue / _chunkSize.TimeValue;
        long timeDifferece = time.TimeValue % _chunkSize.TimeValue;

        Position.TimeValue = time.TimeValue;

        if (timePosition == _currentChunkPosition + 1) {
            await MoveForwardAsync(token);
        }
        else if (timePosition != _currentChunkPosition) {
            await LoadChunksAtAsync((int)timePosition, token);
        }

        UpdatePlaybackPoints(forceEpisodePlayback);

        rewindCompletion.SetResult();
        _rewinding = null;
    }
    public async Task<IPamelloEpisode?> RewindToEpisode(int episodePosition, bool forceEpisodePlayback = true) {
        var episode = Song.Episodes.ElementAtOrDefault(episodePosition);
        if (episode is null) {
            if (episodePosition > 0) {
                await RewindTo(Duration, forceEpisodePlayback, CancellationToken.None);
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
            if (Song.Episodes[i].Start.TotalSeconds <= Position.TotalSeconds &&
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
        
        _currentChunk?.Position = Position.TimeValue % _chunkSize.TimeValue;
            
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
            
        _currentChunk?.Position = Position.TimeValue % _chunkSize.TimeValue;
        
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
        
        var ffmpeg = _dependencies.ResolveRequired("ffmpeg");

        _ffmpeg = Process.Start(new ProcessStartInfo {
            FileName = ffmpeg.GetFile().FullName,
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
        //StaticLogger.Log($"->\n-> loading chunk at {position}\n->");

        if (_ffmpeg is null) {
            //StaticLogger.Log("ffmpeg creating 1");
            CreateFFMpeg();
            //StaticLogger.Log("ffmpeg created 1");
        }
        else {
            _ffmpeg.Resume();
            //_ffmpeg.Resume();
        }

        if (_ffmpeg is null) return null;
        //StaticLogger.Log("ffmpeg is not null");

        long startPos = _chunkSize.TimeValue * position;
        long endPos = _chunkSize.TimeValue * (position + 1);
        //StaticLogger.Log($"Next length: {endPos - startPos}");

        if (_ffmpegPosition != startPos) {
            //StaticLogger.Log("ffmpeg creating 2");
            CreateFFMpeg(new AudioTime(startPos));
            //StaticLogger.Log("ffmpeg created 2");
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
                    //StaticLogger.Log($"<-\n<- loaded CUT chunk at {position}\n<-\n");
                    return chunkStream;
                }
                else {
                    //StaticLogger.Log($"failed to load chunk stream at {position}");
                    return null;
                }
            }

            chunkStream.WriteByte((byte)_ffmpeg.StandardOutput.BaseStream.ReadByte());
            _ffmpegPosition++;
        }
            
        _ffmpeg.Suspend();

        //StaticLogger.Log($"<-\n<- loaded chunk at {position}\n<-\n");
        return chunkStream;
    }

    public static AudioTime? GetSongDuration(IPamelloSong song, IDependenciesService dependencies) {
        if (!(song.SelectedSource?.IsDownloaded() ?? false)) {
            return null;
        }
        
        var ffprobe = dependencies.ResolveRequired("ffprobe");

        using var ffprobeProcess = Process.Start(new ProcessStartInfo {
            FileName = ffprobe.GetFile().FullName,
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

    protected override void InitAudioInternal(IServiceProvider services) {
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
