using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Audio.Attributes;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Services;
using PamelloV7.Core.Audio.Time;
using PamelloV7.Core.DTO;
using PamelloV7.Core.DTO.Other;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.Events;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Extensions;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Module.Marsoau.Base.Queue
{
    public class PamelloQueue : IPamelloQueue, IAudioDependant
    {
        private readonly IServiceProvider _services;

        private readonly IPamelloSongRepository _songs;

        private readonly IPamelloAudioSystem _audio;
        private readonly IEventsService _events;

        [OnAudioMap]
        public IPamelloPlayer Player { get; }

        public bool IsRandom {
            get; set {
                if (field == value) return;

                field = value;

                if (IsRandom) {
                    IsReversed = false;
                }

                _events.Invoke(new PlayerQueueIsRandomUpdated() {
                    Player = Player,
                    IsRandom = IsRandom
                });
            }
        }

        public bool IsReversed {
            get; set {
                if (field == value) return;

                field = value;

                if (IsReversed) {
                    IsRandom = false;
                }

                _events.Invoke(new PlayerQueueIsReversedUpdated() {
                    Player = Player,
                    IsReversed = IsReversed
                });
            }
        }

        public bool IsNoLeftovers {
            get; set {
                if (field == value) return;

                field = value;

                if (!IsNoLeftovers) {
                    IsFeedRandom = false;
                }
                
                _events.Invoke(new PlayerQueueIsNoLeftoversUpdated() {
                    Player = Player,
                    IsNoLeftovers = IsNoLeftovers
                });
            }
        }

        public bool IsFeedRandom {
            get;
            set {
                if (field == value) return;

                field = value;

                if (IsFeedRandom) {
                    IsNoLeftovers = true;

                    if (_entries.Count == 0) {
                        GoToNextSong();
                    }
                }
                
                _events.Invoke(new PlayerQueueIsFeedRandomUpdated() {
                    Player = Player,
                    IsFeedRandom = IsFeedRandom
                });
            }
        }

        private int? _nextPositionRequest;
        public int? NextPositionRequest {
            get => _nextPositionRequest;
            set {
                if (_nextPositionRequest == value) return;

                _nextPositionRequest = value;

                _events.Invoke(new PlayerQueueNextPositionRequestUpdated() {
                    Player = Player,
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

                _events.Invoke(new PlayerQueuePositionUpdated() {
                    Player = Player,
                    Position = Position
                });
            }
        }

        public int? EpisodePosition => _songAudio?.GetCurrentEpisodePosition();

        private SongAudio? _songAudio;
        public IPamelloSong? CurrentSong => _songAudio?.Song;
        public IPamelloEpisode? CurrentEpisode => _songAudio?.GetCurrentEpisode();
        
        public AudioTime CurrentSongTimePosition => _songAudio?.Position ?? new AudioTime(0);
        public AudioTime CurrentSongTimeTotal => _songAudio?.Duration ?? new AudioTime(0);

        private readonly List<PamelloQueueEntry> _entries;
        public IReadOnlyList<PamelloQueueEntry> Entries => _entries;
        public int Count => _entries.Count;

        public IReadOnlyList<IPamelloSong> Songs => _entries.Select(entry => entry.Song).ToList();

        public IEnumerable<PamelloQueueEntryDTO> EntriesDTOs
            => _entries.Select(entry => new PamelloQueueEntryDTO() {
                SongId = entry.Song.Id,
                AdderId = entry.Adder?.Id
            });
        
        public IEnumerable<int> SongsIds => Songs.Select(song => song.Id);

        public PamelloQueue(
            IPamelloPlayer player,
            IServiceProvider services
        ) {
            _services = services;
            
            _audio = services.GetRequiredService<IPamelloAudioSystem>();
            _events = services.GetRequiredService<IEventsService>();
            _songs = services.GetRequiredService<IPamelloSongRepository>();

            Player = player;
            
            IsNoLeftovers = true;

            _entries = [];
        }

        public void SetCurrent(PamelloQueueEntry? entry) {
            if (_songAudio is not null) {
                _audio.DeleteModule(_songAudio);
            }
            
            if (entry?.Song is null) {
                _songAudio = null;
            }
            else {
                _songAudio = _audio.RegisterModule(new SongAudio(entry.Song, _services));
                if (Player.Pump is IAudioModuleWithInput pump) {
                    pump.Input.ConnectedPoint = _songAudio.Output;
                }
                
                //if (!await _songAudio.TryInitialize()) await SetCurrent(null);
            }

            if (_songAudio is not null) {
                SubscribeCurrentAudioEvents();
            }

            //if (entry?.Adder is not null) entry.Adder.SongsPlayed++;

            _events.Invoke(new PlayerQueueCurrentSongIdUpdated() {
                Player = Player,
                CurrentSongId = entry?.Song?.Id
            });

            Current_Position_OnSecondTick();
            Current_Duration_OnSecondTick();
        }

        public Task RewindCurrent(AudioTime toTime) {
            if (_songAudio is null) throw new PamelloException("No current song to rewind");
            
            return _songAudio.RewindTo(toTime);
        }

        public int? RequestNextPosition(string? positionValue) {
            if (positionValue is null) {
                return NextPositionRequest = null;
            }
            
            return NextPositionRequest = TranslateQueuePosition(positionValue);
        }

        public void SubscribeCurrentAudioEvents() {
            if (_songAudio is null) return;

            _songAudio.Position.OnSecondTick = Current_Position_OnSecondTick;
            _songAudio.Duration.OnSecondTick = Current_Duration_OnSecondTick;
            _songAudio.OnEnded = Current_OnEnded;
        }

        private void Current_Duration_OnSecondTick() {
            _events.Invoke(new PlayerQueueCurrentSongTimeTotalUpdated() {
                Player = Player,
                CurrentSongTimeTotal = _songAudio?.Duration.TotalSeconds ?? 0
            });
        }
        private void Current_Position_OnSecondTick() {
            _events.Invoke(new PlayerQueueCurrentSongTimePassedUpdated() {
                Player = Player,
                CurrentSongTimePassed = _songAudio?.Position.TotalSeconds ?? 0
            });
        }
        private void Current_OnEnded()
        {
            GoToNextSong();
        }


		private int TranslateQueuePosition(string positionValue, bool includeLastEmpty = false) {
            var index = _entries.TranslateValueIndex(positionValue, includeLastEmpty, _ => _position);
            return index >= 0 ? index : throw new PamelloException($"Position by value \"{positionValue}\" not found");
		}

        public IPamelloSong? SongAt(int position)
            => _entries.ElementAtOrDefault(position)?.Song;

        public IEnumerable<IPamelloSong> AddSongs(IEnumerable<IPamelloSong> songs, IPamelloUser? adder)
            => InsertSongs((_entries.Count + 1).ToString(), songs, adder);
        public IEnumerable<IPamelloPlaylist> AddPlaylist(IEnumerable<IPamelloPlaylist> playlists, IPamelloUser? adder)
            => InsertPlaylist((_entries.Count + 1).ToString(), playlists, adder);

        public IEnumerable<IPamelloSong> InsertSongs(string position, IEnumerable<IPamelloSong> songs, IPamelloUser? adder) {
            var insertPosition = TranslateQueuePosition(position, true);
            var beforeCount = _entries.Count;
            
            _entries.InsertRange(insertPosition, songs.Select(song => new PamelloQueueEntry(song, adder)));

            _events.Invoke(new PlayerQueueEntriesDTOsUpdated() {
                Player = Player,
                EntriesDTOs = EntriesDTOs
            });

			if (beforeCount == 0 && _entries.Count > 0) {
				SetCurrent(_entries.FirstOrDefault());
                Position = 0;
			}
			else if (insertPosition <= Position) Position++;

            return songs;
		}

        public IEnumerable<IPamelloPlaylist> InsertPlaylist(string positionValue, IEnumerable<IPamelloPlaylist> playlists, IPamelloUser? adder) {
            if (!playlists.Any()) return [];

			var insertPosition = TranslateQueuePosition(positionValue, true);

            var queueWasEmpty = _entries.Count == 0;
            var positionMustBeMoved = insertPosition <= Position;

            var playlistsSongs = playlists.SelectMany(playlist => playlist.Songs).ToList();
            foreach (var song in playlistsSongs) {
				_entries.Insert(insertPosition++, new PamelloQueueEntry(song, adder));
            }

            _events.Invoke(new PlayerQueueEntriesDTOsUpdated() {
                Player = Player,
                EntriesDTOs = EntriesDTOs
            });

			if (queueWasEmpty) {
				SetCurrent(_entries.FirstOrDefault());
            }
			else if (positionMustBeMoved) {
				Position += playlistsSongs.Count;
            }

            return playlists;
        }

        public IPamelloSong RemoveSong(string positionValue) {
            if (_entries.Count == 0) throw new PamelloException("Queue is empty");

            IPamelloSong? song;
			if (_entries.Count == 1) {
				song = _entries.First().Song;
				Clear();
                return song;
			}

			var songPosition = TranslateQueuePosition(positionValue);
			song = _entries[songPosition].Song;
			
			if (Position == songPosition) GoToNextSong(true);
			else {
                _entries.RemoveAt(songPosition);

                if (songPosition < Position) Position--;
                if (songPosition < NextPositionRequest) NextPositionRequest--;
                else if (songPosition == NextPositionRequest) NextPositionRequest = null;

                _events.Invoke(new PlayerQueueEntriesDTOsUpdated() {
                    Player = Player,
                    EntriesDTOs = EntriesDTOs
                });
            }

            return song;
		}
		public bool MoveSong(string fromPositionValue, string toPositionValue) {
			if (_entries.Count < 2) return false;

			var fromPosition = TranslateQueuePosition(fromPositionValue);
			var toPosition = TranslateQueuePosition(toPositionValue, true);

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

            _events.Invoke(new PlayerQueueEntriesDTOsUpdated() {
                Player = Player,
                EntriesDTOs = EntriesDTOs
            });

            return true;
		}
		public bool SwapSongs(string inPositionValue, string withPositionValue) {
			if (_entries.Count < 2) return false;

			var inPosition = TranslateQueuePosition(inPositionValue);
			var withPosition = TranslateQueuePosition(withPositionValue);

			if (inPosition == withPosition) return true;

			(_entries[inPosition], _entries[withPosition]) = (_entries[withPosition], _entries[inPosition]);

            _events.Invoke(new PlayerQueueEntriesDTOsUpdated() {
                Player = Player,
                EntriesDTOs = EntriesDTOs
            });

			if (inPosition == Position) Position = withPosition;
			else if (withPosition == Position) Position = inPosition;

            return true;
		}
		public IPamelloSong GoToSong(string songPositionValue, bool returnBack = false) {
			if (_entries.Count == 0) throw new PamelloException("Queue is empty");

			var nextPosition = TranslateQueuePosition(songPositionValue);
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

                var song = _songs.GetRandom(null!).FirstOrDefault();
                
                if (song is not null) return AddSongs([song], null).FirstOrDefault();
                
                IsFeedRandom = false;
                
                return null;
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

			if (forceRemoveCurrentSong || IsNoLeftovers && _songAudio is not null) {
                _entries.RemoveAt(Position);
				if (nextPosition > Position) nextPosition--;

                //not sure about this invoking
                _events.Invoke(new PlayerQueueEntriesDTOsUpdated() {
                    Player = Player,
                    EntriesDTOs = EntriesDTOs
                });
            }

            if (_entries.Count == 0) Position = 0;
            else Position = nextPosition % _entries.Count;

			if (_entries.Count == 0) {
                SetCurrent(null);
                return null;
			}

            var entry = _entries[Position];
            SetCurrent(entry);

			return entry.Song;
		}

        public async Task<IPamelloEpisode?> GoToEpisode(string episodePositionValue) {
            if (_songAudio is null) throw new PamelloException("No current song to rewind to episode");
            
            var episodePosition = _songAudio.Song.Episodes.TranslateValueIndex(
                episodePositionValue,
                false,
                _ => _songAudio.GetCurrentEpisodePosition()
            );
            
            return await _songAudio.RewindToEpisode(episodePosition);
        }

        public void Clear() {
			_entries.Clear();

            SetCurrent(null);
            Position = 0;

            _events.Invoke(new PlayerQueueEntriesDTOsUpdated() {
                Player = Player,
                EntriesDTOs = EntriesDTOs
            });
        }

        public PamelloQueueDTO GetDto() {
            return new PamelloQueueDTO {
                CurrentSongId = CurrentSong?.Id,
                CurrentSongTimePassed = _songAudio?.Position.TotalSeconds ?? 0,
                CurrentSongTimeTotal = _songAudio?.Duration.TotalSeconds ?? 0,
                EntriesDTOs = EntriesDTOs,
                Position = Position,
                NextPositionRequest = NextPositionRequest,
                CurrentEpisodePosition = _songAudio?.GetCurrentEpisodePosition(),
                
                IsRandom = IsRandom,
                IsReversed = IsReversed,
                IsNoLeftovers = IsNoLeftovers,
                IsFeedRandom = IsFeedRandom,
            };
        }

        public void Dispose() {
            Clear();
        }
    }
}
