using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Audio.Modules;
using PamelloV7.Core.Audio.Attributes;
using PamelloV7.Core.Audio.Modules.Base;
using PamelloV7.Core.Audio.Services;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.EventsOld;
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

                //TODO events here
            }
        }

        public bool IsReversed {
            get; set {
                if (field == value) return;

                field = value;

                if (IsReversed) {
                    IsRandom = false;
                }

                //TODO events here
            }
        }

        public bool IsNoLeftovers {
            get; set {
                if (field == value) return;

                field = value;

                if (!IsNoLeftovers) {
                    IsFeedRandom = false;
                }

                //TODO events here
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

                //TODO events here
            }
        }

        private int? _nextPositionRequest;
        public int? NextPositionRequest {
            get => _nextPositionRequest;
            set {
                if (_nextPositionRequest == value) return;

                _nextPositionRequest = value;

                //TODO events here
            }
        }

        private int _position;
        public int Position {
            get => _position;
            set {
                if (_position == value) return;

                _position = value;

                //TODO events here
            }
        }

        private SongAudio? _songAudio;
        public IPamelloSong? CurrentSong => _songAudio?.Song;
        public IPamelloEpisode? CurrentEpisode => null; //TODO actually return episode here

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

            _entries = [];
        }

        public async Task SetCurrent(PamelloQueueEntry? entry) {
            if (_songAudio is not null) {
                UnsubscribeCurrentAudioEvents();
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
                
                if (!await _songAudio.TryInitialize()) await SetCurrent(null);
            }

            if (_songAudio is not null) {
                SubscribeCurrentAudioEvents();
            }

            //if (entry?.Adder is not null) entry.Adder.SongsPlayed++;

            //TODO events here

            Current_Position_OnSecondTick();
            Current_Duration_OnSecondTick();
        }

        public void SubscribeCurrentAudioEvents() {
            if (_songAudio is null) return;

            _songAudio.Position.OnSecondTick += Current_Position_OnSecondTick;
            _songAudio.Duration.OnSecondTick += Current_Duration_OnSecondTick;
            _songAudio.OnEnded += Current_OnEnded;
        }

        public void UnsubscribeCurrentAudioEvents() {
            if (_songAudio is null) return;

            _songAudio.Position.OnSecondTick -= Current_Position_OnSecondTick;
            _songAudio.Duration.OnSecondTick -= Current_Duration_OnSecondTick;
            _songAudio.OnEnded -= Current_OnEnded;
        }

        private void Current_Duration_OnSecondTick() {
            //TODO events here
        }
        private void Current_Position_OnSecondTick() {
            //TODO events here
        }
        private void Current_OnEnded()
        {
            GoToNextSong();
        }


		private int TranslateQueuePosition(string positionValue, bool includeLastEmpty = false) {
            return _entries.TranslateValueIndex(positionValue, includeLastEmpty, _ => _position);
		}

        public IPamelloSong? SongAt(int position)
            => _entries.ElementAtOrDefault(position)?.Song;

        public IPamelloSong AddSong(IPamelloSong song, IPamelloUser? adder)
            => InsertSong(_entries.Count.ToString(), song, adder);
        public IPamelloPlaylist AddPlaylist(IPamelloPlaylist playlist, IPamelloUser? adder)
            => InsertPlaylist(_entries.Count.ToString(), playlist, adder);

        public IPamelloSong InsertSong(string position, IPamelloSong song, IPamelloUser? adder) {
            Debug.Assert(song is not null, "Null song was inserted into queue");

            var insertPosition = TranslateQueuePosition(position, true);
            _entries.Insert(insertPosition, new PamelloQueueEntry(song, adder));

            //TODO events here

			if (_entries.Count == 1) {
				SetCurrent(_entries.FirstOrDefault());
                Position = 0;
			}
			else if (insertPosition <= Position) Position++;

            return song;
		}

        public IPamelloPlaylist InsertPlaylist(string positionValue, IPamelloPlaylist playlist, IPamelloUser? adder) {
            if (playlist is null) return null;

			var insertPosition = TranslateQueuePosition(positionValue, true);

            var queueWasEmpty = _entries.Count == 0;
            var positionMustBeMoved = insertPosition <= Position;

            var playlistSongs = playlist.Songs;
            foreach (var song in playlistSongs) {
				_entries.Insert(insertPosition++, new PamelloQueueEntry(song, adder));
            }

            //TODO events here

			if (queueWasEmpty) {
				SetCurrent(_entries.FirstOrDefault());
            }
			else if (positionMustBeMoved) {
				Position += playlistSongs.Count;
            }

            return playlist;
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

                //TODO events here
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

            //TODO events here

            return true;
		}
		public bool SwapSongs(string inPositionValue, string withPositionValue) {
			if (_entries.Count < 2) return false;

			var inPosition = TranslateQueuePosition(inPositionValue);
			var withPosition = TranslateQueuePosition(withPositionValue);

			if (inPosition == withPosition) return true;

			(_entries[inPosition], _entries[withPosition]) = (_entries[withPosition], _entries[inPosition]);

            //TODO events here

			if (inPosition == Position) Position = withPosition;
			else if (withPosition == Position) Position = inPosition;

            return true;
		}
		public async Task<IPamelloSong> GoToSong(string songPositionValue, bool returnBack = false) {
			if (_entries.Count == 0) throw new PamelloException("Queue is empty");

			var nextPosition = TranslateQueuePosition(songPositionValue);
			if (returnBack && Position != nextPosition) NextPositionRequest = Position;

            Position = nextPosition;
            var entry = _entries[Position];

            await SetCurrent(entry);

            return entry.Song;
		}
		public async Task<IPamelloSong?> GoToNextSong(bool forceRemoveCurrentSong = false) {
			if (_entries.Count == 0)
            {
                if (!IsFeedRandom) return null;

                var song = _songs.GetRandom(null!).FirstOrDefault();
                
                if (song is not null) return AddSong(song, null);
                
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

                //TODO events here
            }

			Position = TranslateQueuePosition(nextPosition.ToString());

			if (_entries.Count == 0) {
                await SetCurrent(null);
                return null;
			}

            var entry = _entries[Position];
            await SetCurrent(entry);

			return entry.Song;
		}


		public void Clear() {
			_entries.Clear();

            SetCurrent(null);
            Position = 0;

            //TODO events here
        }

        public void Dispose() {
            Clear();
        }
    }
}
