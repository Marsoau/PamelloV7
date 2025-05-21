using PamelloV7.Core.DTO;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Modules.Basic;
using PamelloV7.Server.Model.Audio.Speakers;
using PamelloV7.Server.Model.Discord;
using PamelloV7.Server.Model.Listeners;
using PamelloV7.Server.Repositories.Database;
using PamelloV7.Server.Repositories.Dynamic;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Model.Audio
{
    public class PamelloPlayer : IPamelloEntity, IAudioModuleWithModel, IDisposable
    {
        private readonly IServiceProvider _services;
        
        private readonly PamelloSongRepository _songs;
        private readonly PamelloUserRepository _users;
        
        private readonly PamelloSpeakerRepository _speakerRepository;
        private readonly PamelloEventsService _events;

        public readonly PamelloUser Creator;
        public readonly PamelloSpeakerCollection Speakers;

        public AudioModel Model { get; }

        private PamelloAudio _testSongAudio;
        private AudioPump _pump;
        private PamelloInternetSpeaker _testSpeaker;

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

        public bool IsDisposed { get; private set; }

        public readonly PamelloQueue Queue;

        private static int _idCounter = 1;

        public PamelloPlayer(IServiceProvider services,
            string name,
            PamelloUser creator
        ) {
            _services = services;
            
            _songs = services.GetRequiredService<PamelloSongRepository>();
            _users = services.GetRequiredService<PamelloUserRepository>();
            
            _speakerRepository = services.GetRequiredService<PamelloSpeakerRepository>();
            _events = services.GetRequiredService<PamelloEventsService>();

            Id = _idCounter++;
            _name = name;
            _status = EPlayerState.AwaitingSong;

            _isProtected = true;

            Creator = creator;

            Queue = new PamelloQueue(services, this);
            Speakers = new PamelloSpeakerCollection(services, this);

            Model = new AudioModel();
            //Task.Run(MusicRestartingLoop);
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
                if (!Speakers.IsAnyAvailable()) {
                    State = EPlayerState.AwaitingSpeaker;

                    await Task.Delay(250);
                    continue;
				}
				if (IsPaused) {
					await Task.Delay(250);
					continue;
				}

				State = EPlayerState.Active;

                success = await Queue.Current.NextBytes(audio, true);

                try {
                    if (success) await Speakers.BroadcastBytes(this, audio);
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

        public void Dispose() {
            IsDisposed = true;
            
            Queue.Dispose();
            Speakers.Dispose();
        }
        public void InitModel() {
            var testSong = _songs.GetRequired(434);
            var testUser = _users.GetRequired(1);
            
            Model.AddModules([
                _testSongAudio = new PamelloAudio(_services, testSong),
                _pump = new AudioPump(48000),
                _testSpeaker = testUser.Commands.SpeakerInternetConnect("test", true).Result
            ]);
        }

        public void InitModule()
        {
            _testSongAudio.TryInitialize().Wait();
            Console.WriteLine("starting player pump");

            Console.WriteLine("before connection:");
            Console.WriteLine($"Pump input: {_pump.Input}");
            Console.WriteLine($"Pump output: {_pump.Output}");
            _pump.Input.ConnectBack(_testSongAudio.Output);
            _pump.Output.ConnectFront(_testSpeaker.Input);
            _pump.Condition = PumpCondition;
            
            Console.WriteLine("after connection:");
            Console.WriteLine($"Pump input: {_pump.Input}");
            Console.WriteLine($"Pump output: {_pump.Output}");

            _ = _pump.Start();
            
            Console.WriteLine("pump started");
        }

        private async Task<bool> PumpCondition()
        {
            Console.WriteLine($"pump condition called. result: {_testSpeaker.ListenersCount > 0}");
            return _testSpeaker.ListenersCount > 0;
            if (Queue.Current is null) {
                State = EPlayerState.AwaitingSong;
                return false;
            }
            if (!Queue.Current.IsInitialized) {
                State = EPlayerState.AwaitingSongInitialization;
                if (!await Queue.Current.TryInitialize()) {
                    Console.WriteLine("failed to initialize");
                    Queue.GoToNextSong(true);
                    return false;
                }
            }
            if (!Speakers.IsAnyAvailable()) {
                State = EPlayerState.AwaitingSpeaker;
                return false;
            }
            if (IsPaused)
            {
                return false;
            }

            return true;
        }
    }
}
