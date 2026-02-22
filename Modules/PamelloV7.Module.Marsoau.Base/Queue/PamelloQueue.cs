using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Audio.Attributes;
using PamelloV7.Framework.Audio.Modules.Base;
using PamelloV7.Framework.Audio.Services;
using PamelloV7.Framework.DTO;
using PamelloV7.Framework.DTO.Other;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Events;
using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Framework.Events.Miscellaneous;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.Extensions;
using PamelloV7.Framework.Repositories;
using PamelloV7.Framework.Services;
using PamelloV7.Framework.Services.PEQL;

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

            _events.Invoke(scopeUser, new PlayerQueueIsRandomUpdated() {
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

            _events.Invoke(scopeUser, new PlayerQueueIsReversedUpdated() {
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
            
            _events.Invoke(scopeUser, new PlayerQueueIsNoLeftoversUpdated() {
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
            
            _events.Invoke(scopeUser, new PlayerQueueIsFeedRandomUpdated() {
                Player = Player,
                IsFeedRandom = IsFeedRandom
            });
        }

        public int? NextPositionRequest { get; private set; }
        public void SetNextPositionRequest(int? position, IPamelloUser? scopeUser) {
            if (NextPositionRequest == position) return;

            NextPositionRequest = position;

            _events.Invoke(scopeUser, new PlayerQueueNextPositionRequestUpdated() {
                Player = Player,
                NextPositionRequest = NextPositionRequest
            });
        }

        public int Position { get; private set; }
        public void SetPosition(int position, IPamelloUser? scopeUser) {
            if (Position == position) return;

            Position = position;

            _events.Invoke(scopeUser, new PlayerQueuePositionUpdated() {
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
            }

            if (_songAudio is not null) {
                SubscribeCurrentAudioEvents();
            }

            //if (entry?.Adder is not null) entry.Adder.SongsPlayed++;

            _events.Invoke(null, new PlayerQueueCurrentSongIdUpdated() {
                Player = Player,
                CurrentSongId = entry?.Song?.Id
            });

            Current_Position_OnSecondTick();
            Current_Duration_OnSecondTick();

            if (_songAudio is not null) _ = Task.Run(async () => {
                if (!await _songAudio.TryInitialize()) GoToNextSong(null, true);
            });
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
            _events.Invoke(null, new PlayerQueueCurrentSongTimeTotalUpdated() {
                Player = Player,
                CurrentSongTimeTotal = _songAudio?.Duration.TotalSeconds ?? 0
            });
        }
        private void Current_Position_OnSecondTick() {
            _events.Invoke(null, new PlayerQueueCurrentSongTimePassedUpdated() {
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

        public IEnumerable<IPamelloSong> InsertSongs(string position, IEnumerable<IPamelloSong> esongs, IPamelloUser? adder) {
            var songs = esongs.ToList();
            
            var insertPosition = TranslateQueuePosition(position, true);
            var beforeCount = _entries.Count;
            
            _entries.InsertRange(insertPosition, songs.Select(song => new PamelloQueueEntry(song, adder)));

            _events.Invoke(adder, new SongAddedToQueue() {
                Player = Player,
                Entries = new List<PamelloQueueEntry>(Entries),
                AddedSongs = songs,
                QueuePosition = insertPosition
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

            _events.Invoke(adder, new PlayerQueueEntriesUpdated() {
                Player = Player,
                Entries = Entries
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

                _events.Invoke(scopeUser, new PlayerQueueEntriesUpdated() {
                    Player = Player,
                    Entries = Entries
                });
            }

            return song;
		}

        public int RemoveSongsRange(string fromPositionValue, string toPositionValue, IPamelloUser? scopeUser) {
            var fromPosition = TranslateQueuePosition(fromPositionValue);
            var toPosition = TranslateQueuePosition(toPositionValue);
            
            if (fromPosition > toPosition) (fromPosition, toPosition) = (toPosition, fromPosition);
            
            var removedCount = toPosition - fromPosition + 1;
            if (removedCount > _entries.Count) removedCount = _entries.Count;

            _entries.RemoveRange(fromPosition, removedCount);

            if (Position >= fromPosition && Position <= toPosition) {
                Position = fromPosition;
                if (Position > 0) Position--;
                
                SetPosition(Position, null);
                SetCurrent(Entries.ElementAtOrDefault(Position), null);
            }

            _events.Invoke(scopeUser, new PlayerQueueEntriesUpdated() {
                Player = Player,
                Entries = Entries
            });

            return removedCount;
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

            _events.Invoke(scopeUser, new PlayerQueueEntriesUpdated() {
                Player = Player,
                Entries = Entries
            });

            return true;
		}

        public bool SwapSongs(string inPositionValue, string withPositionValue, IPamelloUser? scopeUser) {
			if (_entries.Count < 2) return false;

			var inPosition = TranslateQueuePosition(inPositionValue);
			var withPosition = TranslateQueuePosition(withPositionValue);

			if (inPosition == withPosition) return true;

			(_entries[inPosition], _entries[withPosition]) = (_entries[withPosition], _entries[inPosition]);

            _events.Invoke(scopeUser, new PlayerQueueEntriesUpdated() {
                Player = Player,
                Entries = Entries
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
                _events.Invoke(scopeUser, new PlayerQueueEntriesUpdated() {
                    Player = Player,
                    Entries = Entries
                });
            }

            if (_entries.Count == 0) Position = 0;
            else Position = nextPosition % _entries.Count;

			if (_entries.Count == 0) {
                if (IsFeedRandom) {
                    var song = _songs.GetRandom(null!).FirstOrDefault();
                    
                    if (song is not null) return AddSongs([song], null).FirstOrDefault();
                    
                    IsFeedRandom = false;
                }
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

            _events.Invoke(scopeUser, new PlayerQueueEntriesUpdated() {
                Player = Player,
                Entries = Entries
            });
        }

        public PamelloQueueDTO GetDto() {
            return new PamelloQueueDTO {
                CurrentSongId = CurrentSong?.Id,
                CurrentSongTimePassed = _songAudio?.Position.TotalSeconds ?? 0,
                CurrentSongTimeTotal = _songAudio?.Duration.TotalSeconds ?? 0,
                Entries = Entries,
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
