using PamelloV7.Core.Audio;
using PamelloV7.Core.Enumerators;
using PamelloV7.Server.Extensions;
using PamelloV7.Server.Services;
using System.Diagnostics;

namespace PamelloV7.Server.Model.Audio
{
    public class PamelloAudio
    {
        private readonly YoutubeDownloadService _downloader;

        public readonly PamelloSong Song;

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

        public PamelloAudio(IServiceProvider services,
            PamelloSong song
        ) {
            _downloader = services.GetRequiredService<YoutubeDownloadService>();

            Song = song;

            Position = new AudioTime(0);
            Duration = new AudioTime(0);

            _chunkSize = new AudioTime(8);
        }

		public void Clean() {
            _currentChunk?.Dispose();
            _currentChunk = null;
            _nextChunk?.Dispose();
            _nextChunk = null;

            _nextBreakPoint = null;
            _nextJumpPoint = null;

            Position.TimeValue = 0;
			Duration.TimeValue = 0;
		}

        public async Task<bool> TryInitialize() {
            if (IsInitialized) return true;

            if (!Song.IsDownloaded) {
                if (await _downloader.DownloadFromYoutubeAsync(Song) != EDownloadResult.Success) {
                    Clean();
                    return false;
                }
            }

            Duration.TotalSeconds = GetSongDuration(Song)?.TotalSeconds ?? 0;

            await LoadChunksAtAsync(0);
            if (_currentChunk is null) return false;

            _currentChunkPosition = 0;

            return true;
        }

		public async Task<bool> NextBytes(byte[] result) {
            if (_rewinding is not null) await _rewinding;
			if (_currentChunk is null) return false;

            if (Position.TimeValue >= Duration.TimeValue) {
                return false;
            }

			if (Position.TotalSeconds == _nextBreakPoint) {
                if (_nextJumpPoint is null) {
                    await RewindTo(Duration);
                    return false;
                }

				await RewindTo(new AudioTime(_nextJumpPoint.Value));
            }
            
            if (Position.TimeValue / _chunkSize.TimeValue > _currentChunkPosition) {
                await MoveForwardAsync();
            }

            _currentChunk.Position = Position.TimeValue % _chunkSize.TimeValue;
			if (_currentChunk.Read(result, 0, 2) == 2) {
				Position.TimeValue += 2;

				return true;
			}

			return false;
		}

        public void UpdatePlaybackPoints(bool excludeCurrent = false) {
            int? closestBreakPoint = null;
            int? closestJumpPoint = null;

            foreach (var episode in Song.Episodes) {
				if (
					(excludeCurrent) ?
					(episode.Start > Position.TotalSeconds) :
					(episode.Start >= Position.TotalSeconds)
				) {
					if (episode.Skip) {
                        if (closestBreakPoint is null || episode.Start < closestBreakPoint) {
                            closestBreakPoint = episode.Start;
                        }
                    }
					else if (episode.Start > closestBreakPoint) {
                        if (closestJumpPoint is null || episode.Start < closestJumpPoint) {
                            closestJumpPoint = episode.Start;
                        }
                    }
				}
			}

            _nextBreakPoint = closestBreakPoint;
            _nextJumpPoint = closestJumpPoint;

            //Console.WriteLine($"Updated playback points: [{_nextBreakPoint}] | [{_nextJumpPoint}]");
        }

        public async Task RewindTo(AudioTime time, bool forceEpisodePlayback = true) {
            var rewindCompletion = new TaskCompletionSource();
            while (_rewinding is not null) {
                await _rewinding;
            }

            _rewinding = rewindCompletion.Task;

            if (_currentChunk is null) return;

            long timePosition = time.TimeValue / _chunkSize.TimeValue;
            long timeDifferece = time.TimeValue % _chunkSize.TimeValue;

            if (timePosition == _currentChunkPosition + 1) {
                await MoveForwardAsync();
            }
            else if (timePosition != _currentChunkPosition) {
                await LoadChunksAtAsync((int)timePosition);
            }

            Position.TimeValue = time.TimeValue;

            UpdatePlaybackPoints(forceEpisodePlayback);

            rewindCompletion.SetResult();
            _rewinding = null;
        }
        public async Task RewindToEpisode(int episodePosition) {
            if (episodePosition >= Song.Episodes.Count) {
                await RewindTo(Duration);
                return;
            }

            var episode = Song.Episodes.ElementAtOrDefault(episodePosition);
            if (episode is null) return;

            await RewindTo(new AudioTime(episode.Start));
        }

        public int? GetCurrentEpisodePosition() {
            int? closestLeft = null;

            for (int i = 0; i < Song.Episodes.Count; i++) {
                if (Song.Episodes[i].Start < Position.Seconds && Song.Episodes[i].Start > (closestLeft ?? int.MinValue)) {
                    closestLeft = Song.Episodes[i].Start;
                }
            }

            return closestLeft;
        }
        public PamelloEpisode? GetCurrentEpisode() {
            var currentPosition = GetCurrentEpisodePosition();
            if (currentPosition is null) return null;

            return Song.Episodes.ElementAtOrDefault(currentPosition.Value);
        }

        public async Task LoadChunksAtAsync(int position) {
            _currentChunkPosition = position;
            _currentChunk = await LoadChunkAsync(position);
            _nextChunk = await LoadChunkAsync(position + 1);
        }
        private async Task MoveForwardAsync() {
            _currentChunk = _nextChunk;
            _nextChunk = await LoadChunkAsync(++_currentChunkPosition + 1);
        }

        private void CreateFFMpeg() {
            if (_ffmpeg is not null) {
                _ffmpeg.Dispose();
                _ffmpeg = null;
                _ffmpegPosition = 0;
            }

			_ffmpeg = Process.Start(new ProcessStartInfo {
				FileName = "ffmpeg",
				Arguments = $@"-speed ultrafast -hide_banner -loglevel panic -i ""{GetSongAudioPath(Song)}"" -ac 2 -f s16le -ar 48000 pipe:1",
				UseShellExecute = false,
				RedirectStandardOutput = true
			});
        }

        private async Task<MemoryStream?> LoadChunkAsync(int position) {
            Console.WriteLine($"->\n-> loading chunk at {position}\n->");


            if (_ffmpeg is null) {
                Console.WriteLine("ffmpeg creating 1");
                CreateFFMpeg();
            }
            else {
                _ffmpeg.Resume();
            }

            if (_ffmpeg is null) return null;

            long startPos = _chunkSize.TimeValue * position;
            long endPos = _chunkSize.TimeValue * (position + 1);

            byte[] buffer = new byte[2];

            if (_ffmpegPosition > startPos) {
                Console.WriteLine("ffmpeg creating 2");
                CreateFFMpeg();
            }

            Console.WriteLine($"seeking to the start from {_ffmpegPosition} to {startPos}");
            while (_ffmpegPosition < startPos) {
                if (_ffmpeg.StandardOutput.EndOfStream) return null;

                _ffmpeg.StandardOutput.BaseStream.ReadByte();
                _ffmpeg.StandardOutput.BaseStream.ReadByte();
                _ffmpegPosition += 2;
            }

            var chunkStream = new MemoryStream();

            Console.WriteLine($"seeking to the end from {_ffmpegPosition} to {endPos}");
            while (_ffmpegPosition < endPos) {
                if (_ffmpeg.StandardOutput.EndOfStream) {
                    if (chunkStream.Length > 0) {
                        Console.WriteLine($"<-\n<- loaded CUT chunk at {position}\n<-\n");
                        return chunkStream;
                    }
                    else return null;
                }

                _ffmpeg.StandardOutput.BaseStream.Read(buffer);
                chunkStream.Write(buffer);
                _ffmpegPosition += 2;
            }

            _ffmpeg.Suspend();

            Console.WriteLine($"<-\n<- loaded chunk at {position}\n<-\n");
            return chunkStream;
        }

        public static string GetSongAudioPath(PamelloSong song)
            => $@"{AppContext.BaseDirectory}Data\Music\{song.Id}.opus";
        public static AudioTime? GetSongDuration(PamelloSong song) {
            if (!song.IsDownloaded) {
                return null;
			}

			using var ffprobeProcess = Process.Start(new ProcessStartInfo {
				FileName = "ffprobe",
				Arguments = $@"-i ""{GetSongAudioPath(song)}"" -show_entries format=duration -v quiet -of csv=""p=0""",
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
    }
}
