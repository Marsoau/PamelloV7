using PamelloV7.Core.DTO;
using PamelloV7.Core.Enumerators;
using PamelloV7.Server.Model.Discord;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Model.Audio
{
    public class PamelloPlayer : IEntity
    {
        private readonly YoutubeDownloadService _downloader;
        private readonly PamelloSpeakerService _speakers;

        public readonly PamelloUser Creator;

        public int Id { get; }

        private string _name;
        public string Name {
            get => _name;
        }

        private EPlayerStatus _status;
        public EPlayerStatus Status {
            get => _status;
            set {
                if (_status == value) return;

                _status = value;

                Console.WriteLine($"status changed to {_status}");
                //event
            }
        }

        private bool _isProtected;
        public bool IsProtected {
            get => _isProtected;
            set {
                if (_isProtected == value) return;

                _isProtected = value;
                //event
            }
        }

        private bool _isPaused;
        public bool IsPaused {
            get => _isPaused;
            set => _isPaused = value;
        }

        public readonly PamelloQueue Queue;
        public IReadOnlyList<PamelloUser> Users {
            get => throw new NotImplementedException();
        }

        public object? DTO => throw new NotImplementedException();

        private static int _idCounter = 1;

        public PamelloPlayer(IServiceProvider services,
            string name,
            PamelloUser creator
        ) {
            _downloader = services.GetRequiredService<YoutubeDownloadService>();
            _speakers = services.GetRequiredService<PamelloSpeakerService>();

            Id = _idCounter++;
            _name = name;
            _status = EPlayerStatus.AwaitingSong;

            _isProtected = true;

            Creator = creator;

            Queue = new PamelloQueue(services, this);

            Task.Run(MusicRestartingLoop);
        }

        public async Task MusicRestartingLoop() {
            while (true) {
                try {
                    await MusicLoop();
                }
                catch (Exception x) {
                    Console.WriteLine($"Exceprion occured in music loop: {x.Message}");
                    Console.WriteLine(x);
                }
            }
            Console.WriteLine("MusicLoop ended");
        }
        public async Task MusicLoop() {
            byte[] audio = new byte[2];
            bool success = false;

            if (Queue.Current is null) return;
            if (!await Queue.Current.TryInitialize()) return;

            while (true) {
                if (Queue.Current is null) {
                    Status = EPlayerStatus.AwaitingSong;
                    
                    await Task.Delay(250);
                    continue;
                }
                if (!Queue.Current.IsInitialized) {
                    Status = EPlayerStatus.AwaitingSongInitialization;
                    if (!await Queue.Current.TryInitialize()) {
                        Queue.GoToNextSong(true);
                    }
                }
                if (!_speakers.IsChannelActive(Id)) {
                    Status = EPlayerStatus.AwaitingSpeaker;

                    await Task.Delay(250);
                    continue;
                }
                if (IsPaused) {
                    await Task.Delay(250);
                    continue;
                }

                Status = EPlayerStatus.Active;

                success = await Queue.Current.NextBytes(audio);

                try {
                    if (success) await _speakers.BroadcastBytes(Id, audio);
                    else {
                        Console.WriteLine($"{Queue.Current.Song} ended");
                    
                        Queue.GoToNextSong();
                    }
                } catch (Exception x) {
                    Console.WriteLine($"Play bytes exception: {x}");
                }
            }
        }

        public override string ToString() {
            return $"[{Id}] {Name}";
        }

        public DiscordString ToDiscordString() {
            return DiscordString.Bold(Name) + " " + DiscordString.Code($"[{Id}]");
        }
        public string ToDiscordFooterString() {
            return $"{Name} [{Id}] {(IsProtected ? " (private)" : "")}";
        }

        public IPamelloDTO GetDTO() {
            return new PamelloPlayerDTO {
                Id = Id,
                Name = Name,
                IsPaused = IsPaused,
                State = Status,

                CurrentSongId = Queue.Current?.Song.Id,
                QueueSongsIds = Queue.Songs.Select(song => song.Id),
                QueuePosition = Queue.Position,
                CurrentEpisodePosition = Queue.Current?.GetCurrentEpisodePosition(),
                NextPositionRequest = Queue.NextPositionRequest,

                CurrentSongTimePassed = Queue.Current?.Position.TotalSeconds ?? 0,
                CurrentSongTimeTotal = Queue.Current?.Duration.TotalSeconds ?? 0,
                
                QueueIsRandom = Queue.IsRandom,
                QueueIsReversed = Queue.IsReversed,
                QueueIsNoLeftovers = Queue.IsNoLeftovers,
                QueueIsFeedRandom = Queue.IsFeedRandom,
            };
        }
    }
}
