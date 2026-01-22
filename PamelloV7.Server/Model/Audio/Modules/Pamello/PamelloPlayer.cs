using Discord.WebSocket;
using PamelloV7.Core.Audio;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.EventsOld;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Repositories;
using PamelloV7.Server.Model.Audio.Interfaces;
using PamelloV7.Server.Model.Audio.Modules.Basic;
using PamelloV7.Server.Model.Audio.Modules.Inputs;
using PamelloV7.Server.Model.Audio.Speakers;
using PamelloV7.Server.Model.Discord;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Model.Audio.Modules.Pamello
{
    public class PamelloPlayer : IPamelloPlayer, IAudioModuleWithModel
    {
        private readonly IServiceProvider _services;
        
        private readonly IPamelloSongRepository _songs;
        private readonly IPamelloUserRepository _users;
        
        private readonly IPamelloSpeakerRepository _speakerRepository;
        private readonly SSEBroadcastService _events;

        public IPamelloUser Creator { get; }

        public AudioModel ParentModel { get; }
        public AudioModel Model { get; }

        private AudioPump _pump;
        private AudioCopy _speakersCopy;

        public int Id { get; }

        private string _name;
        public string Name {
            get => _name;
            set => throw new NotImplementedException();
        }

        public bool IsChangesGoing => throw new NotImplementedException();
        public void StartChanges() {
            throw new NotImplementedException();
        }

        public void EndChanges() {
            throw new NotImplementedException();
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

        public IPamelloQueue Queue { get; private set; }

        private static int _idCounter = 1;

        public PamelloPlayer(
            AudioModel parentModel,
            IServiceProvider services,
            
            string name,
            IPamelloUser creator
        ) {
            ParentModel = parentModel;
            _services = services;
            
            _songs = services.GetRequiredService<IPamelloSongRepository>();
            _users = services.GetRequiredService<IPamelloUserRepository>();
            
            _speakerRepository = services.GetRequiredService<IPamelloSpeakerRepository>();
            _events = services.GetRequiredService<SSEBroadcastService>();

            Id = _idCounter++;
            _name = name;
            _status = EPlayerState.AwaitingSong;

            _isProtected = true;

            Creator = creator;

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
            /*
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
            */
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

        public IPamelloDTO GetDto() {
            return new PamelloPlayerDTO {
                Id = Id,
                Name = Name,
                IsPaused = IsPaused,
                State = State,

                OwnerId = Creator.Id,
                IsProtected = IsProtected,

                CurrentSongId = Queue.Audio?.Song.Id,
                QueueEntriesDTOs = Queue.EntriesDTOs,
                QueuePosition = Queue.Position,
                CurrentEpisodePosition = Queue.Audio?.GetCurrentEpisodePosition(),
                NextPositionRequest = Queue.NextPositionRequest,

                CurrentSongTimePassed = Queue.Audio?.Position.TotalSeconds ?? 0,
                CurrentSongTimeTotal = Queue.Audio?.Duration.TotalSeconds ?? 0,
                
                QueueIsRandom = Queue.IsRandom,
                QueueIsReversed = Queue.IsReversed,
                QueueIsNoLeftovers = Queue.IsNoLeftovers,
                QueueIsFeedRandom = Queue.IsFeedRandom,
            };
        }

        public void Init() {
        }

        public void Dispose() {
            IsDisposed = true;
            
            Model.Dispose();
        }
        public void InitModel() {
            Model.AddModules([
                (PamelloQueue)(Queue = new PamelloQueue(Model, _services, this)),
                _pump = new AudioPump(Model, 48000),
                _speakersCopy = new AudioCopy(Model, false),
            ]);
            
            _pump.Input.ConnectBack(((PamelloQueue)Queue).Output);
            _pump.Output.ConnectFront(_speakersCopy.Input);
            _pump.Condition = async () => !IsPaused;
        }

        public void InitModule()
        {
            _pump.Start();
            Console.WriteLine("player pump started");
        }
        
        public async Task<IPamelloInternetSpeaker> AddInternet(string? name) {
            //if (!_repository.IsInternetChannelAvailable(channel)) throw new PamelloException($"Channel \"{channel}\" is not available");
        
            var internetSpeaker = Model.AddModule(new PamelloInternetSpeaker(Model, this, name));
            //await internetSpeaker.InitialConnection();
            _speakersCopy.CreateOutput().ConnectFront(internetSpeaker.Input);

            return internetSpeaker;
        }
        /*
        public async Task<PamelloDiscordSpeaker?> AddDiscord(DiscordSocketClient client, ulong guildId, ulong vcId) {
            var guild = client.GetGuild(guildId);
            if (guild is null) return null;

            var speaker = Model.AddModule(
                new PamelloDiscordSpeaker(_services, client, guild.Id, this)
            );
            await speaker.InitialConnect(vcId);

            _speakersCopy.CreateOutput().ConnectFront(speaker.Input);

            return speaker;
        }
        */
    }
}
