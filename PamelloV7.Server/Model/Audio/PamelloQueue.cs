using Discord;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Model.Discord;
using PamelloV7.Server.Model.Interactions.Builders;
using PamelloV7.Server.Repositories;
using System.Text;

namespace PamelloV7.Server.Model.Audio
{
    public class PamelloQueue
    {
        private readonly IServiceProvider _services;

        private readonly PamelloPlayer _player;

        private readonly PamelloUserRepository _users;
        private readonly PamelloSongRepository _songs;
        private readonly List<PamelloSong> _audios;

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
                    IsFeedRandom = false;
                }
            }
        }
        public bool IsReversed {
            get => _isReversed;
            set {
                if (_isReversed == value) return;

                _isReversed = value;

                if (IsReversed) {
                    IsRandom = false;
                    IsFeedRandom = false;
                }
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
            }
        }
        public bool IsFeedRandom {
            get => _isFeedRandom;
            set {
                if (_isFeedRandom == value) return;

                _isFeedRandom = value;

                if (IsFeedRandom) {
                    IsNoLeftovers = true;

                    if (_audios.Count == 0) {
                        GoToNextSong();
                    }
                }
            }
        }

        private int? _nextPositionRequest;
        public int? NextPositionRequest {
            get => _nextPositionRequest;
            set => _nextPositionRequest = value;
        }

        private int _position;
        public int Position {
            get => _position;
            set => _position = value;
        }

        private PamelloAudio? _current;
        public PamelloAudio? Current {
            get => _current;
        }

        public int Count {
            get => _audios.Count;
        }
        public IReadOnlyList<PamelloSong> Songs {
            get => _audios;
        }

        public void SetCurrent(PamelloSong? song) {
            if (_current is not null) {
                UnsubscribeCurrentAudioEvents();
            }

            _current?.Clean();
            if (song is null) {
                _current = null;
            }
            else {
                var audio = new PamelloAudio(_services, song);
                _current = audio;
            }

            //event

            if (_current is not null) {
                SubscribeCurrentAudioEvents();
            }

            Current_Position_OnSecondTick();
            Current_Duration_OnSecondTick();
        }

        public PamelloQueue(IServiceProvider services, PamelloPlayer parrentPlayer) {
            _services = services;

            _users = services.GetRequiredService<PamelloUserRepository>();
            _player = parrentPlayer;
            _songs = services.GetRequiredService<PamelloSongRepository>();

            _isRandom = false;
            _isReversed = false;
            _isNoLeftovers = true;

            _audios = new List<PamelloSong>();
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

        }
        private void Current_Position_OnSecondTick() {

        }


		private int NormalizeQueuePosition(int position, bool includeLastEmpty = false) {
			if (_audios.Count == 0) return 0;

			position %= _audios.Count + (includeLastEmpty ? 1 : 0);
			if (position < 0) position += _audios.Count;

			return position;
		}

        public PamelloSong AddSong(PamelloSong song)
            => InsertSong(_audios.Count, song);
        public PamelloPlaylist AddPlaylist(PamelloPlaylist playlist)
            => InsertPlaylist(_audios.Count, playlist);

        public PamelloSong? At(int position)
            => _audios.ElementAtOrDefault(position);

        public PamelloSong InsertSong(int position, PamelloSong song) {
            if (song is null) return null;

            var insertPosition = NormalizeQueuePosition(position, true);
            _audios.Insert(insertPosition, song);

			if (_audios.Count == 1) {
				SetCurrent(_audios.FirstOrDefault());
                Position = 0;
			}
			else if (insertPosition <= Position) Position++;

            return song;
		}

        public PamelloPlaylist InsertPlaylist(int position, PamelloPlaylist playlist) {
            if (playlist is null) return null;

			var insertPosition = NormalizeQueuePosition(position, true);

            var queueWasEmpty = _audios.Count == 0;
            var positionMustBeMoved = insertPosition <= Position;

            var playlistSongs = playlist.Songs;
            foreach (var song in playlistSongs) {
				_audios.Insert(insertPosition++, song);
            }

			if (queueWasEmpty) {
				SetCurrent(_audios.FirstOrDefault());
            }
			else if (positionMustBeMoved) {
				Position += playlistSongs.Count;
            }

            return playlist;
        }

        public PamelloSong RemoveSong(int songPosition) {
            if (_audios.Count == 0) throw new PamelloException("Queue is empty");

            PamelloSong? song;
			if (_audios.Count == 1) {
				song = _audios.First();
				Clear();
                return song;
			}

			songPosition = NormalizeQueuePosition(songPosition);
			song = _audios[songPosition];
			
			if (Position == songPosition) GoToNextSong(true);
			else {
                _audios.RemoveAt(songPosition);
                if (songPosition < Position) Position--;
                //event
            }

            return song;
		}
		public bool MoveSong(int fromPosition, int toPosition) {
			if (_audios.Count < 2) return false;

			fromPosition = NormalizeQueuePosition(fromPosition);
			toPosition = NormalizeQueuePosition(toPosition, true);

			if (fromPosition == toPosition) return true;

            var buffer = _audios[fromPosition];
            _audios.RemoveAt(fromPosition);
			if (fromPosition < toPosition) toPosition--;
            _audios.Insert(toPosition, buffer);

            if (fromPosition == Position) Position = toPosition;
            else if (fromPosition < Position && Position <= toPosition) {
				Position--;
			}
			else if (fromPosition > Position && Position >= toPosition) {
                Position++;
            }

            //event

            return true;
		}
		public bool SwapSongs(int inPosition, int withPosition) {
			if (_audios.Count < 2) return false;

			inPosition = NormalizeQueuePosition(inPosition);
			withPosition = NormalizeQueuePosition(withPosition);

			if (inPosition == withPosition) return true;

			var buffer = _audios[inPosition];
			_audios[inPosition] = _audios[withPosition];
			_audios[withPosition] = buffer;

            //event

			if (inPosition == Position) Position = withPosition;
			else if (withPosition == Position) Position = inPosition;

            return true;
		}
		public PamelloSong GoToSong(int songPosition, bool returnBack = false) {
			if (_audios.Count == 0) throw new PamelloException("Queue is empty");

			var nextPosition = NormalizeQueuePosition(songPosition);
			if (returnBack && Position != nextPosition) NextPositionRequest = Position;

            Position = nextPosition;
            var song = _audios[Position];

            SetCurrent(song);

            return song;
		}
		public PamelloSong? GoToNextSong(bool forceRemoveCurrentSong = false) {
            PamelloSong? song = null;

			if (_audios.Count == 0) {
                if (IsFeedRandom) {
                    return AddSong(_songs.GetRandomPV5(_users.GetRequired(1)).Result);
                }
                else return null;
            }

            int nextPosition;

			if (NextPositionRequest is not null) {
				nextPosition = NextPositionRequest.Value;
				NextPositionRequest = null;
			}
			else if (IsRandom && _audios.Count > 1) do {
                nextPosition = Random.Shared.Next(0, _audios.Count);
            } while (Position == nextPosition);
			else if (IsReversed) nextPosition = Position - 1;
			else nextPosition = Position + 1;

			if (forceRemoveCurrentSong || IsNoLeftovers && Current is not null) {
                _audios.RemoveAt(Position);
				if (nextPosition > Position) nextPosition--;

                //event
            }

			Position = NormalizeQueuePosition(nextPosition);

			if (_audios.Count == 0) {
                if (IsFeedRandom) {
                    return AddSong(_songs.GetRandomPV5(_users.GetRequired(1)).Result);
                }
                else {
                    SetCurrent(null);
                    return null;
                }
			}

            song = _audios[Position];
            SetCurrent(song);

			return song;
		}

        public EmbedBuilder QueuePageBuilder(int page, int elementCount) {
            var embedBuilder = PamelloEmbedBuilder.Page(
                "Queue",
                _audios,
                (sb, pos, song) => {
                    sb.Append(DiscordString.Code(pos));
                    if (Position == pos) {
                        sb.Append(
                            (" > " + song.ToDiscordString()).Bold()
                        );
                    }
                    else {
                        sb.Append(" - ");
                        sb.Append(song.ToDiscordString());
                    }

                    sb.AppendLine();
                }
            );
            embedBuilder.Footer.Text += $" | {_player.ToDiscordFooterString()}";

            return embedBuilder;
        }

		public void Clear() {
			_audios.Clear();

            SetCurrent(null);
            Position = 0;

            //event
        }
    }
}
