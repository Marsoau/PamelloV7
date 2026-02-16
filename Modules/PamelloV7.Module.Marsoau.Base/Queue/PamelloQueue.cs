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

        public bool IsRandom { get; private set; }
        public void SetIsRandom(bool state, IPamelloUser? scopeUser) {
            if (IsRandom == state) return;

            IsRandom = state;

            if (IsRandom) {
                IsReversed = false;
            }

            _events.InvokeAsync(scopeUser, new PlayerQueueIsRandomUpdated() {
                Player = Player,
                IsRandom = IsRandom
            });
        }

        public bool IsReversed { get; private set; }
        public void SetIsReversed(bool state, IPamelloUser? scopeUser) {
            if (IsReversed == state) return;

            IsReversed = state;

            if (IsReversed) {
                IsRandom = false;
            }

            _events.InvokeAsync(scopeUser, new PlayerQueueIsReversedUpdated() {
                Player = Player,
                IsReversed = IsReversed
            });
        }

        public bool IsNoLeftovers { get; private set; }
        public void SetIsNoLeftovers(bool state, IPamelloUser? scopeUser) {
            if (IsNoLeftovers == state) return;

            IsNoLeftovers = state;

            if (!IsNoLeftovers) {
                IsFeedRandom = false;
            }
            
            _events.InvokeAsync(scopeUser, new PlayerQueueIsNoLeftoversUpdated() {
                Player = Player,
                IsNoLeftovers = IsNoLeftovers
            });
        }

        public bool IsFeedRandom { get; private set; }
        public void SetIsFeedRandom(bool state, IPamelloUser? scopeUser) {
            if (IsFeedRandom == state) return;

            IsFeedRandom = state;

            if (IsFeedRandom) {
                IsNoLeftovers = true;

                if (_entries.Count == 0) {
                    GoToNextSong();
                }
            }
            
            _events.InvokeAsync(scopeUser, new PlayerQueueIsFeedRandomUpdated() {
                Player = Player,
                IsFeedRandom = IsFeedRandom
            });
        }

        public int? NextPositionRequest { get; private set; }
        public void SetNextPositionRequest(int? position, IPamelloUser? scopeUser) {
            if (NextPositionRequest == position) return;

            NextPositionRequest = position;

            _events.InvokeAsync(scopeUser, new PlayerQueueNextPositionRequestUpdated() {
                Player = Player,
                NextPositionRequest = NextPositionRequest
            });
        }

        public int Position { get; private set; }
        public void SetPosition(int position, IPamelloUser? scopeUser) {
            if (Position == position) return;

            Position = position;

            _events.InvokeAsync(scopeUser, new PlayerQueuePositionUpdated() {
                Player = Player,
                Position = Position
            });
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

        public void SetCurrent(PamelloQueueEntry? entry, IPamelloUser? scopeUser) {
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

            _events.InvokeAsync(null, new PlayerQueueCurrentSongIdUpdated() {
                Player = Player,
                CurrentSongId = entry?.Song?.Id
            });

            Current_Position_OnSecondTick();
            Current_Duration_OnSecondTick();
        }

        public Task RewindCurrent(AudioTime toTime, IPamelloUser? scopeUser) {
            if (_songAudio is null) throw new PamelloException("No current song to rewind");
            
            return _songAudio.RewindTo(toTime);
        }

        public int? RequestNextPosition(string? positionValue, IPamelloUser? scopeUser) {
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
            _events.InvokeAsync(null, new PlayerQueueCurrentSongTimeTotalUpdated() {
                Player = Player,
                CurrentSongTimeTotal = _songAudio?.Duration.TotalSeconds ?? 0
            });
        }
        private void Current_Position_OnSecondTick() {
            _events.InvokeAsync(null, new PlayerQueueCurrentSongTimePassedUpdated() {
                Player = Player,
                CurrentSongTimePassed = _songAudio?.Position.TotalSeconds ?? 0
            });
        }
        private void Current_OnEnded()
        {
            GoToNextSong();
        }


		private int TranslateQueuePosition(string positionValue, bool includeLastEmpty = false) {
            var index = _entries.TranslateValueIndex(positionValue, includeLastEmpty, _ => Position);
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

            _events.InvokeAsync(adder, new PlayerQueueEntriesDTOsUpdated() {
                Player = Player,
                EntriesDTOs = EntriesDTOs
            });

			if (beforeCount == 0 && _entries.Count > 0) {
				SetCurrent(_entries.FirstOrDefault(), adder);
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

            _events.InvokeAsync(adder, new PlayerQueueEntriesDTOsUpdated() {
                Player = Player,
                EntriesDTOs = EntriesDTOs
            });

			if (queueWasEmpty) {
				SetCurrent(_entries.FirstOrDefault(), adder);
            }
			else if (positionMustBeMoved) {
				Position += playlistsSongs.Count;
            }

            return playlists;
        }

        public IPamelloSong RemoveSong(string positionValue, IPamelloUser? scopeUser) {
            if (_entries.Count == 0) throw new PamelloException("Queue is empty");

            IPamelloSong? song;
			if (_entries.Count == 1) {
				song = _entries.First().Song;
				Clear();
                return song;
			}

			var songPosition = TranslateQueuePosition(positionValue);
			song = _entries[songPosition].Song;
			
			if (Position == songPosition) GoToNextSong(forceRemoveCurrentSong: true);
			else {
                _entries.RemoveAt(songPosition);

                if (songPosition < Position) Position--;
                if (songPosition < NextPositionRequest) NextPositionRequest--;
                else if (songPosition == NextPositionRequest) NextPositionRequest = null;

                _events.InvokeAsync(scopeUser, new PlayerQueueEntriesDTOsUpdated() {
                    Player = Player,
                    EntriesDTOs = EntriesDTOs
                });
            }

            return song;
		}
		public bool MoveSong(string fromPositionValue, string toPositionValue, IPamelloUser? scopeUser) {
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

            _events.InvokeAsync(scopeUser, new PlayerQueueEntriesDTOsUpdated() {
                Player = Player,
                EntriesDTOs = EntriesDTOs
            });

            return true;
		}

        public bool SwapSongs(string inPositionValue, string withPositionValue, IPamelloUser? scopeUser) {
			if (_entries.Count < 2) return false;

			var inPosition = TranslateQueuePosition(inPositionValue);
			var withPosition = TranslateQueuePosition(withPositionValue);

			if (inPosition == withPosition) return true;

			(_entries[inPosition], _entries[withPosition]) = (_entries[withPosition], _entries[inPosition]);

            _events.InvokeAsync(scopeUser, new PlayerQueueEntriesDTOsUpdated() {
                Player = Player,
                EntriesDTOs = EntriesDTOs
            });

			if (inPosition == Position) Position = withPosition;
			else if (withPosition == Position) Position = inPosition;

            return true;
		}
		public IPamelloSong GoToSong(string songPositionValue, IPamelloUser? scopeUser, bool returnBack = false) {
			if (_entries.Count == 0) throw new PamelloException("Queue is empty");

			var nextPosition = TranslateQueuePosition(songPositionValue);
			if (returnBack && Position != nextPosition) NextPositionRequest = Position;

            Position = nextPosition;
            var entry = _entries[Position];

            SetCurrent(entry, scopeUser);

            return entry.Song;
		}
		public IPamelloSong? GoToNextSong(IPamelloUser? scopeUser = null, bool forceRemoveCurrentSong = false) {
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
                _events.InvokeAsync(scopeUser, new PlayerQueueEntriesDTOsUpdated() {
                    Player = Player,
                    EntriesDTOs = EntriesDTOs
                });
            }

            if (_entries.Count == 0) Position = 0;
            else Position = nextPosition % _entries.Count;

			if (_entries.Count == 0) {
                SetCurrent(null, scopeUser);
                return null;
			}

            var entry = _entries[Position];
            SetCurrent(entry, scopeUser);

			return entry.Song;
		}

        public async Task<IPamelloEpisode?> GoToEpisode(string episodePositionValue, IPamelloUser? scopeUser) {
            if (_songAudio is null) throw new PamelloException("No current song to rewind to episode");
            
            var episodePosition = _songAudio.Song.Episodes.TranslateValueIndex(
                episodePositionValue,
                false,
                _ => _songAudio.GetCurrentEpisodePosition()
            );
            
            return await _songAudio.RewindToEpisode(episodePosition);
        }

        public void Clear(IPamelloUser? scopeUser = null) {
			_entries.Clear();

            SetCurrent(null, scopeUser);
            Position = 0;

            _events.InvokeAsync(scopeUser, new PlayerQueueEntriesDTOsUpdated() {
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
