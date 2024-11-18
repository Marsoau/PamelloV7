using PamelloV7.Server.Exceptions;
using PamelloV7.Server.Repositories;

namespace PamelloV7.Server.Model.Audio
{
    public class PamelloQueue
    {
        private readonly IServiceProvider _services;

        private readonly PamelloPlayer _player;

        private readonly PamelloSongRepository _songs;
        private readonly List<PamelloAudio> _audios;

        private bool _isRandom;
        private bool _isReversed;
        private bool _isNoLeftovers;

        public bool IsRandom {
            get => _isRandom;
        }
        public bool IsReversed {
            get => _isReversed;
        }
        public bool IsNoLeftovers{
            get => _isNoLeftovers;
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
            private set {
                if (_current is not null) {
                    UnsubscribeCurrentAudioEvents();
                }

                _current?.Clean();
                _current = value;

                //event

                if (_current is not null) {
                    SubscribeCurrentAudioEvents();
                }

                Current_Position_OnSecondTick();
                Current_Duration_OnSecondTick();
            }
        }

        public PamelloQueue(IServiceProvider services, PamelloPlayer parrentPlayer) {
            _services = services;

            _player = parrentPlayer;
            _songs = services.GetRequiredService<PamelloSongRepository>();

            _isRandom = false;
            _isReversed = false;
            _isNoLeftovers = true;

            _audios = new List<PamelloAudio>();
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
            Console.WriteLine($"tick {Current?.Position}");
        }


		private int NormalizeQueuePosition(int position, bool includeLastEmpty = false) {
			if (_audios.Count == 0) return 0;

			position %= _audios.Count + (includeLastEmpty ? 1 : 0);
			if (position < 0) position += _audios.Count;

			return position;
		}

        public void AddSong(PamelloSong song)
            => InsertSong(_audios.Count, song);
        public void AddPlaylist(PamelloPlaylist playlist)
            => InsertPlaylist(_audios.Count, playlist);

        public void InsertSong(int position, PamelloSong song) {
            if (song is null) return;

			var songAudio = new PamelloAudio(_services, song);

            var insertPosition = NormalizeQueuePosition(position, true);
            _audios.Insert(insertPosition, songAudio);

			if (_audios.Count == 1) {
				Current = _audios.FirstOrDefault();
                Position = 0;
			}
			else if (insertPosition <= Position) Position++;

            //event
		}

        public void InsertPlaylist(int position, PamelloPlaylist playlist) {
            if (playlist is null) return;

			var insertPosition = NormalizeQueuePosition(position, true);

            var queueWasEmpty = _audios.Count == 0;
            var positionMustBeMoved = insertPosition <= Position;

            var playlistSongs = playlist.Songs;
            foreach (var song in playlistSongs) {
				_audios.Insert(insertPosition++, new PamelloAudio(_services, song));
            }

			if (queueWasEmpty) {
				Current = _audios.FirstOrDefault();
            }
			else if (positionMustBeMoved) {
				Position += playlistSongs.Count;
            }

            //event
        }

        public PamelloSong RemoveSong(int songPosition) {
            if (_audios.Count == 0) throw new PamelloException("Queue is empty");

            PamelloSong? song;
			if (_audios.Count == 1) {
				song = _audios.First().Song;
				Clear();
                return song;
			}

			songPosition = NormalizeQueuePosition(songPosition);
			song = _audios[songPosition].Song;
			
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
		public void GoToSong(int songPosition, bool returnBack = false) {
			if (_audios.Count == 0) throw new PamelloException("Queue is empty");

			var nextPosition = NormalizeQueuePosition(songPosition);
			if (returnBack && Position != nextPosition) NextPositionRequest = Position;

            Position = nextPosition;
			Current = _audios[Position];
		}
		public PamelloSong? GoToNextSong(bool forceRemoveCurrentSong = false) {
			if (_audios.Count == 0) return null;

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

            nextPosition = NormalizeQueuePosition(nextPosition);

			if (_audios.Count == 0) {
				Current = null;
				return null;
			}

			Position = nextPosition;
			Current = _audios[Position];

			return Current.Song;
		}

		public void Clear() {
			_audios.Clear();

			Current = null;
            Position = 0;

            //event
        }
    }
}
