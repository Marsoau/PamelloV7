using PamelloV7.Core.DTO;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using PamelloV7.Server.Model.Discord;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Model.Audio
{
    public class PamelloPlayer : IEntity
    {
        private readonly PamelloSpeakerService _speakers;
        private readonly PamelloEventsService _events;

        public readonly PamelloUser Creator;

        public int Id { get; }

        private string _name;
        public string Name {
            get => _name;
        }

        private EPlayerState _status;
        public EPlayerState State {
            get => _status;
            set {
                if (_status == value) return;

                _status = value;

                _events.BroadcastToPlayer(this, new PlayerStateUpdated() {
                    State = State
                });
            }
        }

        private bool _isProtected;
        public bool IsProtected {
            get => _isProtected;
            set {
                if (_isProtected == value) return;

                _isProtected = value;

                _events.BroadcastToPlayer(this, new PlayerProtectionUpdated() {
                    IsProtected = IsProtected
                });
            }
        }

        private bool _isPaused;
        public bool IsPaused {
            get => _isPaused;
            set {
                if (_isPaused == value) return;

                _isPaused = value;

                _events.BroadcastToPlayer(this, new PlayerIsPausedUpdated() {
                    IsPaused = IsPaused
                });
            }
        }

        public readonly PamelloQueue Queue;

        private static int _idCounter = 1;

        public PamelloPlayer(IServiceProvider services,
            string name,
            PamelloUser creator
        ) {
            _speakers = services.GetRequiredService<PamelloSpeakerService>();
            _events = services.GetRequiredService<PamelloEventsService>();

            Id = _idCounter++;
            _name = name;
            _status = EPlayerState.AwaitingSong;

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

            //if (Queue.Current is null) return;
            //if (!await Queue.Current.TryInitialize()) return;

            while (true) {
                if (Queue.Current is null) {
                    State = EPlayerState.AwaitingSong;

                    await Task.Delay(250);
                    continue;
                }
                if (!Queue.Current.IsInitialized) {
                    State = EPlayerState.AwaitingSongInitialization;
                    if (!await Queue.Current.TryInitialize()) {
						Console.WriteLine("failed to initialize");
                        Queue.GoToNextSong(true);
						continue;
                    }
                }
                if (!_speakers.DoesPlayerHasSpeakers(this)) {
                    State = EPlayerState.AwaitingSpeaker;

                    await Task.Delay(250);
                    continue;
				}
				if (IsPaused) {
					await Task.Delay(250);
					continue;
				}

				State = EPlayerState.Active;

                success = await Queue.Current.NextBytes(audio);

                try {
                    if (success) await _speakers.BroadcastBytes(this, audio);
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
                State = State,

                OwnerId = Creator.Id,
                IsProtected = IsProtected,

                CurrentSongId = Queue.Current?.Song.Id,
                QueueEntriesDTOs = Queue.EntriesDTOs,
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
