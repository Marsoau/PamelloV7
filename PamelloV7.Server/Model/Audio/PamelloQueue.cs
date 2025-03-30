using Discord;
using PamelloV7.Core.Events;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.DTO;
using PamelloV7.Server.Model.Discord;
using PamelloV7.Server.Model.Interactions.Builders;
using PamelloV7.Server.Repositories;
using PamelloV7.Server.Services;

namespace PamelloV7.Server.Model.Audio
{
    public class PamelloQueueEntry {
        public readonly PamelloSong Song;
        public readonly PamelloUser? Adder;

        public PamelloQueueEntry(PamelloSong song, PamelloUser? adder) {
            Song = song;
            Adder = adder;
        }
    }

    public class PamelloQueue
    {
        private readonly IServiceProvider _services;

        private readonly PamelloEventsService _events;

        private readonly PamelloPlayer _player;

        private readonly PamelloUserRepository _users;
        private readonly PamelloSongRepository _songs;
        private readonly List<PamelloQueueEntry> _entries;

        private bool _isRandom;
        private bool _isReversed;
        private bool _isNoLeftovers;
        private bool _isFeedRandom;

        public bool IsRandom {
            get => _isRandom;
            set {
                if (_isRandom == value) return;

                _isRandom = value;

                if (IsRandom) {
                    IsReversed = false;
                }

                _events.BroadcastToPlayer(_player, new PlayerQueueIsRandomUpdated() {
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

                _events.BroadcastToPlayer(_player, new PlayerQueueIsReversedUpdated() {
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

                _events.BroadcastToPlayer(_player, new PlayerQueueIsNoLeftoversUpdated() {
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

                _events.BroadcastToPlayer(_player, new PlayerQueueIsFeedRandomUpdated() {
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

                _events.BroadcastToPlayer(_player, new PlayerNextPositionRequestUpdated() {
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

                _events.BroadcastToPlayer(_player, new PlayerQueuePositionUpdated() {
                    QueuePosition =_position,
                });
            }
        }

        private PamelloAudio? _current;
        public PamelloAudio? Current {
            get => _current;
        }

        public PamelloUser? CurrentAdder { get; private set; }

        public int Count {
            get => _entries.Count;
        }
        public IReadOnlyList<PamelloQueueEntry> Entries {
            get => _entries;
        }
        public IReadOnlyList<PamelloSong> Songs {
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
            if (_current is not null) {
                UnsubscribeCurrentAudioEvents();
            }

            _current?.Clean();
            if (entry?.Song is null) {
                _current = null;
                CurrentAdder = null;
            }
            else {
                _current = new PamelloAudio(_services, entry.Song);
                CurrentAdder = entry.Adder;
            }

            if (_current is not null) {
                SubscribeCurrentAudioEvents();
            }

            _events.BroadcastToPlayer(_player, new PlayerCurrentSongIdUpdated() {
                CurrentSongId = _current?.Song.Id
            });

            Current_Position_OnSecondTick();
            Current_Duration_OnSecondTick();
        }

        public PamelloQueue(IServiceProvider services, PamelloPlayer parrentPlayer) {
            _services = services;

            _users = services.GetRequiredService<PamelloUserRepository>();
            _player = parrentPlayer;
            _songs = services.GetRequiredService<PamelloSongRepository>();
            _events = services.GetRequiredService<PamelloEventsService>();

            _isRandom = false;
            _isReversed = false;
            _isNoLeftovers = true;
            _isFeedRandom = false;

            _entries = new List<PamelloQueueEntry>();
        }

        public void SubscribeCurrentAudioEvents() {
            if (Current is null) return;

            Current.Position.OnSecondTick += Current_Position_OnSecondTick;
            Current.Duration.OnSecondTick += Current_Duration_OnSecondTick;
        }

        public void UnsubscribeCurrentAudioEvents() {
            if (Current is null) return;

            Current.Position.OnSecondTick -= Current_Position_OnSecondTick;
            Current.Duration.OnSecondTick -= Current_Duration_OnSecondTick;
        }

        private void Current_Duration_OnSecondTick() {
			Console.WriteLine($"tick {Current?.Duration.TotalSeconds}");
            _events.BroadcastToPlayer(_player, new PlayerCurrentSongTimeTotalUpdated() {
                CurrentSongTimeTotal = Current?.Duration.TotalSeconds ?? 0,
            });
        }
        private void Current_Position_OnSecondTick() {
            _events.BroadcastToPlayer(_player, new PlayerCurrentSongTimePassedUpdated() {
                CurrentSongTimePassed = Current?.Position.TotalSeconds ?? 0,
            });
        }


		private int NormalizeQueuePosition(int position, bool includeLastEmpty = false) {
			if (_entries.Count == 0) return 0;

			position %= _entries.Count + (includeLastEmpty ? 1 : 0);
			if (position < 0) position += _entries.Count;

			return position;
		}

        public PamelloSong? SongAt(int position)
            => _entries.ElementAtOrDefault(position)?.Song;

        public PamelloSong AddSong(PamelloSong song, PamelloUser? adder)
            => InsertSong(_entries.Count, song, adder);
        public PamelloPlaylist AddPlaylist(PamelloPlaylist playlist, PamelloUser? adder)
            => InsertPlaylist(_entries.Count, playlist, adder);

        public PamelloSong InsertSong(int position, PamelloSong song, PamelloUser? adder) {
            if (song is null) return null;

            var insertPosition = NormalizeQueuePosition(position, true);
            _entries.Insert(insertPosition, new PamelloQueueEntry(song, adder));

            _events.BroadcastToPlayer(_player, new PlayerQueueEntriesDTOsUpdated() {
                QueueEntriesDTOs = EntriesDTOs,
            });

			if (_entries.Count == 1) {
				SetCurrent(_entries.FirstOrDefault());
                Position = 0;
			}
			else if (insertPosition <= Position) Position++;

            return song;
		}

        public PamelloPlaylist InsertPlaylist(int position, PamelloPlaylist playlist, PamelloUser? adder) {
            if (playlist is null) return null;

			var insertPosition = NormalizeQueuePosition(position, true);

            var queueWasEmpty = _entries.Count == 0;
            var positionMustBeMoved = insertPosition <= Position;

            var playlistSongs = playlist.Songs;
            foreach (var song in playlistSongs) {
				_entries.Insert(insertPosition++, new PamelloQueueEntry(song, adder));
            }

            _events.BroadcastToPlayer(_player, new PlayerQueueEntriesDTOsUpdated() {
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

        public PamelloSong RemoveSong(int songPosition) {
            if (_entries.Count == 0) throw new PamelloException("Queue is empty");

            PamelloSong? song;
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

                _events.BroadcastToPlayer(_player, new PlayerQueueEntriesDTOsUpdated() {
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

            _events.BroadcastToPlayer(_player, new PlayerQueueEntriesDTOsUpdated() {
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

            _events.BroadcastToPlayer(_player, new PlayerQueueEntriesDTOsUpdated() {
                QueueEntriesDTOs = EntriesDTOs,
            });

			if (inPosition == Position) Position = withPosition;
			else if (withPosition == Position) Position = inPosition;

            return true;
		}
		public PamelloSong GoToSong(int songPosition, bool returnBack = false) {
			if (_entries.Count == 0) throw new PamelloException("Queue is empty");

			var nextPosition = NormalizeQueuePosition(songPosition);
			if (returnBack && Position != nextPosition) NextPositionRequest = Position;

            Position = nextPosition;
            var entry = _entries[Position];

            SetCurrent(entry);

            return entry.Song;
		}
		public PamelloSong? GoToNextSong(bool forceRemoveCurrentSong = false) {
			if (_entries.Count == 0) {
                if (IsFeedRandom) {
                    return AddSong(_songs.GetRandomPV5(_users.GetRequired(1)).Result, null);
                }
                else return null;
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

			if (forceRemoveCurrentSong || IsNoLeftovers && Current is not null) {
                _entries.RemoveAt(Position);
				if (nextPosition > Position) nextPosition--;

                _events.BroadcastToPlayer(_player, new PlayerQueueEntriesDTOsUpdated() {
                    QueueEntriesDTOs = EntriesDTOs,
                });
            }

			Position = NormalizeQueuePosition(nextPosition);

			if (_entries.Count == 0) {
                if (IsFeedRandom) {
                    return AddSong(_songs.GetRandomPV5(_users.GetRequired(1)).Result, null);
                }
                else {
                    SetCurrent(null);
                    return null;
                }
			}

            var entry = _entries[Position];
            SetCurrent(entry);

			return entry.Song;
		}

        public EmbedBuilder QueuePageBuilder(int page, int elementCount) {
            var embedBuilder = PamelloEmbedBuilder.Page(
                "Queue",
                _entries,
                (sb, pos, entry) => {
                    sb.Append(DiscordString.Code(pos));
                    if (Position == pos) {
                        sb.Append(
                            (" > " + entry.Song.ToDiscordString()).Bold()
                        );
                    }
                    else {
                        sb.Append(" - ");
                        sb.Append(entry.Song.ToDiscordString());
                    }

                    sb.AppendLine();
                }
            );
            embedBuilder.Footer.Text += $" | {_player.ToDiscordFooterString()}";

            return embedBuilder;
        }

		public void Clear() {
			_entries.Clear();

            SetCurrent(null);
            Position = 0;

            _events.BroadcastToPlayer(_player, new PlayerQueueEntriesDTOsUpdated() {
                QueueEntriesDTOs = EntriesDTOs,
            });
        }
    }
}
