using System.Diagnostics;
using PamelloV7.Core.AudioOld;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Entities;
using PamelloV7.Core.EventsOld;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Repositories;
using PamelloV7.Server.Model.AudioOld.Interfaces;
using PamelloV7.Server.Model.AudioOld.Modules.Inputs;
using PamelloV7.Server.Model.AudioOld.Points;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Model.AudioOld.Modules.Pamello
{
    public class PamelloQueue :IPamelloQueue, IAudioModuleWithModel, IAudioModuleWithOutputs<AudioPullPoint>
    {
        private readonly IServiceProvider _services;

        private readonly SSEBroadcastService _events;

        public IPamelloPlayerOld Player { get; }

        private readonly IPamelloUserRepository _users;
        private readonly IPamelloSongRepository _songs;
        private readonly List<PamelloQueueEntry> _entries;

        private bool _isRandom;
        private bool _isReversed;
        private bool _isNoLeftovers;
        private bool _isFeedRandom;
        
        public AudioModel ParentModel { get; }
        public AudioModel Model { get; }

        public int MinOutputs => 1;
        public int MaxOutputs => 1;

        public AudioPullPoint Output;

        public bool IsDisposed { get; private set; }

        public bool IsRandom {
            get => _isRandom;
            set {
                if (_isRandom == value) return;

                _isRandom = value;

                if (IsRandom) {
                    IsReversed = false;
                }

                _events.BroadcastToPlayer(Player, new PlayerQueueIsRandomUpdated() {
                    IsRandom = IsRandom,
                });
            }
        }
        public bool IsReversed {
            get => _isReversed;
            set {
                if (_isReversed == value) return;

                _isReversed = value;

                if (IsReversed) {
                    IsRandom = false;
                }

                _events.BroadcastToPlayer(Player, new PlayerQueueIsReversedUpdated() {
                    IsReversed = IsReversed,
                });
            }
        }
        public bool IsNoLeftovers {
            get => _isNoLeftovers;
            set {
                if (_isNoLeftovers == value) return;

                _isNoLeftovers = value;

                if (!IsNoLeftovers) {
                    IsFeedRandom = false;
                }

                _events.BroadcastToPlayer(Player, new PlayerQueueIsNoLeftoversUpdated() {
                    IsNoLeftovers = IsNoLeftovers,
                });
            }
        }
        public bool IsFeedRandom {
            get => _isFeedRandom;
            set {
                if (_isFeedRandom == value) return;

                _isFeedRandom = value;

                if (IsFeedRandom) {
                    IsNoLeftovers = true;

                    if (_entries.Count == 0) {
                        GoToNextSong();
                    }
                }

                _events.BroadcastToPlayer(Player, new PlayerQueueIsFeedRandomUpdated() {
                    QueueIsFeedRandom = IsFeedRandom,
                });
            }
        }

        private int? _nextPositionRequest;
        public int? NextPositionRequest {
            get => _nextPositionRequest;
            set {
                if (_nextPositionRequest == value) return;

                _nextPositionRequest = value;

                _events.BroadcastToPlayer(Player, new PlayerNextPositionRequestUpdated() {
                    NextPositionRequest = NextPositionRequest
                });
            }
        }

        private int _position;
        public int Position {
            get => _position;
            set {
                if (_position == value) return;

                _position = value;

                _events.BroadcastToPlayer(Player, new PlayerQueuePositionUpdated() {
                    QueuePosition =_position,
                });
            }
        }

        private IAudioSong? _audio;
        public IAudioSong? Audio {
            get => _audio;
        }

        public int Count {
            get => _entries.Count;
        }
        public IReadOnlyList<PamelloQueueEntry> Entries {
            get => _entries;
        }
        public IReadOnlyList<IPamelloSong> Songs {
            get => _entries.Select(entry => entry.Song).ToList();
        }
        public IEnumerable<PamelloQueueEntryDTO> EntriesDTOs {
            get => _entries.Select(entry => new PamelloQueueEntryDTO() {
                SongId = entry.Song.Id,
                AdderId = entry.Adder?.Id
            });
        }
        public IEnumerable<int> SongsIds {
            get => Songs.Select(song => song.Id);
        }

        public void SetCurrent(PamelloQueueEntry? entry) {
            if (_audio is not null) {
                UnsubscribeCurrentAudioEvents();
            }

            _audio?.Dispose();
            
            if (entry?.Song is null) {
                _audio = null;
            }
            else {
                AudioSong audio;
                _audio = audio = Model.AddModule(new AudioSong(Model, _services, entry.Song));
                audio.Output.ConnectFront(Output);
                
                _ = Task.Run(async () =>
                {
                    if (!await _audio.TryInitialize()) SetCurrent(null);
                });
            }

            if (_audio is not null) {
                SubscribeCurrentAudioEvents();
            }

            //if (entry?.Adder is not null) entry.Adder.SongsPlayed++;

            _events.BroadcastToPlayer(Player, new PlayerCurrentSongIdUpdated() {
                CurrentSongId = _audio?.Song.Id
            });

            Current_Position_OnSecondTick();
            Current_Duration_OnSecondTick();
        }

        public PamelloQueue(
            AudioModel parentModel,
            IServiceProvider services,
            IPamelloPlayerOld parrentPlayer
        ) {
            _services = services;

            _users = services.GetRequiredService<IPamelloUserRepository>();
            Player = parrentPlayer;
            _songs = services.GetRequiredService<IPamelloSongRepository>();
            _events = services.GetRequiredService<SSEBroadcastService>();

            _isRandom = false;
            _isReversed = false;
            _isNoLeftovers = true;
            _isFeedRandom = false;

            _entries = new List<PamelloQueueEntry>();
            
            ParentModel = parentModel;
            Model = new AudioModel();
        }

        public void InitModel()
        {
        }

        public AudioPullPoint CreateOutput()
        {
            Output = new AudioPullPoint(this);
            
            return Output;
        }

        public void InitModule()
        {
        }

        public void SubscribeCurrentAudioEvents() {
            if (Audio is null) return;

            Audio.Position.OnSecondTick += Current_Position_OnSecondTick;
            Audio.Duration.OnSecondTick += Current_Duration_OnSecondTick;
            Audio.OnEnded += Current_OnEnded;
        }

        public void UnsubscribeCurrentAudioEvents() {
            if (Audio is null) return;

            Audio.Position.OnSecondTick -= Current_Position_OnSecondTick;
            Audio.Duration.OnSecondTick -= Current_Duration_OnSecondTick;
            Audio.OnEnded -= Current_OnEnded;
        }

        private void Current_Duration_OnSecondTick() {
			//Console.WriteLine($"tick {Current?.Duration.TotalSeconds}");
            _events.BroadcastToPlayer(Player, new PlayerCurrentSongTimeTotalUpdated() {
                CurrentSongTimeTotal = Audio?.Duration.TotalSeconds ?? 0,
            });
        }
        private void Current_Position_OnSecondTick() {
            _events.BroadcastToPlayer(Player, new PlayerCurrentSongTimePassedUpdated() {
                CurrentSongTimePassed = Audio?.Position.TotalSeconds ?? 0,
            });
        }
        private void Current_OnEnded()
        {
            GoToNextSong();
        }


        public static int NormalizePosition(int position, int size, bool includeLastEmpty = false) {
			if (size == 0) return 0;

			position %= size + (includeLastEmpty ? 1 : 0);
			if (position < 0) position += size;

			return position;
        }
		private int NormalizeQueuePosition(int position, bool includeLastEmpty = false) {
			return NormalizePosition(position, _entries.Count, includeLastEmpty);
		}

        public IPamelloSong? SongAt(int position)
            => _entries.ElementAtOrDefault(position)?.Song;

        public IPamelloSong AddSong(IPamelloSong song, IPamelloUser? adder)
            => InsertSong(_entries.Count, song, adder);
        public IPamelloPlaylist AddPlaylist(IPamelloPlaylist playlist, IPamelloUser? adder)
            => InsertPlaylist(_entries.Count, playlist, adder);

        public IPamelloSong InsertSong(int position, IPamelloSong song, IPamelloUser? adder) {
            Debug.Assert(song is not null, "Null song was inserted into queue");

            var insertPosition = NormalizeQueuePosition(position, true);
            _entries.Insert(insertPosition, new PamelloQueueEntry(song, adder));

            _events.BroadcastToPlayer(Player, new PlayerQueueEntriesDTOsUpdated() {
                QueueEntriesDTOs = EntriesDTOs,
            });

			if (_entries.Count == 1) {
				SetCurrent(_entries.FirstOrDefault());
                Position = 0;
			}
			else if (insertPosition <= Position) Position++;

            return song;
		}

        public IPamelloPlaylist InsertPlaylist(int position, IPamelloPlaylist playlist, IPamelloUser? adder) {
            if (playlist is null) return null;

			var insertPosition = NormalizeQueuePosition(position, true);

            var queueWasEmpty = _entries.Count == 0;
            var positionMustBeMoved = insertPosition <= Position;

            var playlistSongs = playlist.Songs;
            foreach (var song in playlistSongs) {
				_entries.Insert(insertPosition++, new PamelloQueueEntry(song, adder));
            }

            _events.BroadcastToPlayer(Player, new PlayerQueueEntriesDTOsUpdated() {
                QueueEntriesDTOs = EntriesDTOs,
            });

			if (queueWasEmpty) {
				SetCurrent(_entries.FirstOrDefault());
            }
			else if (positionMustBeMoved) {
				Position += playlistSongs.Count;
            }

            return playlist;
        }

        public IPamelloSong RemoveSong(int songPosition) {
            if (_entries.Count == 0) throw new PamelloException("Queue is empty");

            IPamelloSong? song;
			if (_entries.Count == 1) {
				song = _entries.First().Song;
				Clear();
                return song;
			}

			songPosition = NormalizeQueuePosition(songPosition);
			song = _entries[songPosition].Song;
			
			if (Position == songPosition) GoToNextSong(true);
			else {
                _entries.RemoveAt(songPosition);

                if (songPosition < Position) Position--;
                if (songPosition < NextPositionRequest) NextPositionRequest--;
                else if (songPosition == NextPositionRequest) NextPositionRequest = null;

                _events.BroadcastToPlayer(Player, new PlayerQueueEntriesDTOsUpdated() {
                    QueueEntriesDTOs = EntriesDTOs,
                });
            }

            return song;
		}
		public bool MoveSong(int fromPosition, int toPosition) {
			if (_entries.Count < 2) return false;

			fromPosition = NormalizeQueuePosition(fromPosition);
			toPosition = NormalizeQueuePosition(toPosition, true);

			if (fromPosition == toPosition) return true;

            var buffer = _entries[fromPosition];
            _entries.RemoveAt(fromPosition);
			if (fromPosition < toPosition) toPosition--;
            _entries.Insert(toPosition, buffer);

            if (fromPosition == Position) Position = toPosition;
            else if (fromPosition < Position && Position <= toPosition) {
				Position--;
			}
			else if (fromPosition > Position && Position >= toPosition) {
                Position++;
            }

            _events.BroadcastToPlayer(Player, new PlayerQueueEntriesDTOsUpdated() {
                QueueEntriesDTOs = EntriesDTOs,
            });

            return true;
		}
		public bool SwapSongs(int inPosition, int withPosition) {
			if (_entries.Count < 2) return false;

			inPosition = NormalizeQueuePosition(inPosition);
			withPosition = NormalizeQueuePosition(withPosition);

			if (inPosition == withPosition) return true;

			var buffer = _entries[inPosition];
			_entries[inPosition] = _entries[withPosition];
			_entries[withPosition] = buffer;

            _events.BroadcastToPlayer(Player, new PlayerQueueEntriesDTOsUpdated() {
                QueueEntriesDTOs = EntriesDTOs,
            });

			if (inPosition == Position) Position = withPosition;
			else if (withPosition == Position) Position = inPosition;

            return true;
		}
		public IPamelloSong GoToSong(int songPosition, bool returnBack = false) {
			if (_entries.Count == 0) throw new PamelloException("Queue is empty");

			var nextPosition = NormalizeQueuePosition(songPosition);
			if (returnBack && Position != nextPosition) NextPositionRequest = Position;

            Position = nextPosition;
            var entry = _entries[Position];

            SetCurrent(entry);

            return entry.Song;
		}
		public IPamelloSong? GoToNextSong(bool forceRemoveCurrentSong = false) {
			if (_entries.Count == 0)
            {
                if (!IsFeedRandom) return null;
                
                return AddSong(_songs.GetRandom(null).First(), null);
            }

            int nextPosition;

			if (NextPositionRequest is not null) {
				nextPosition = NextPositionRequest.Value;
				NextPositionRequest = null;
			}
			else if (IsRandom && _entries.Count > 1) do {
                nextPosition = Random.Shared.Next(0, _entries.Count);
            } while (Position == nextPosition);
			else if (IsReversed) nextPosition = Position - 1;
			else nextPosition = Position + 1;

			if (forceRemoveCurrentSong || IsNoLeftovers && Audio is not null) {
                _entries.RemoveAt(Position);
				if (nextPosition > Position) nextPosition--;

                _events.BroadcastToPlayer(Player, new PlayerQueueEntriesDTOsUpdated() {
                    QueueEntriesDTOs = EntriesDTOs,
                });
            }

			Position = NormalizeQueuePosition(nextPosition);

			if (_entries.Count == 0) {
                if (IsFeedRandom) {
                    return AddSong(_songs.GetRandom(null).First(), null);
                }
                
                SetCurrent(null);
                return null;
			}

            var entry = _entries[Position];
            SetCurrent(entry);

			return entry.Song;
		}


		public void Clear() {
			_entries.Clear();

            SetCurrent(null);
            Position = 0;

            _events.BroadcastToPlayer(Player, new PlayerQueueEntriesDTOsUpdated() {
                QueueEntriesDTOs = EntriesDTOs,
            });
        }

        public void Dispose() {
            Clear();
        }
    }
}
