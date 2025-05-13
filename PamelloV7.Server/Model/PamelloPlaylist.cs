using Microsoft.EntityFrameworkCore;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Events;
using PamelloV7.Core.Exceptions;
using PamelloV7.DAL;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Server.Model.Difference;

namespace PamelloV7.Server.Model
{
    public class PamelloPlaylist : PamelloEntity<DatabasePlaylist>
    {
        private string _name;
        private bool _isProtected;
        private PamelloUser _owner;

        public override string Name {
            get => _name;
            set {
                if (_name == value) return;

                _name = value;
                Save();

                _events.Broadcast(new PlaylistNameUpdated() {
                    PlaylistId = Id,
                    Name = Name
                });
            }
        }

        public bool IsProtected {
            get => _isProtected;
            set {
                if (_isProtected == value) return;

                _isProtected = value;
                Save();

                _events.Broadcast(new PlaylistProtectionUpdated() {
                    PlaylistId = Id,
                    IsProtected = IsProtected
                });
            }
        }

        public PamelloUser OwnedBy {
            get => _owner;
        }

        private List<PamelloSong> _songs;
        private List<PamelloUser> _favoritedBy;

        public IReadOnlyList<PamelloSong> Songs {
            get => _songs;
        }
        public IReadOnlyList<PamelloUser> FavoritedBy {
            get => _favoritedBy;
        }

        public IEnumerable<int> SongsIds {
            get => _songs.Select(song => song.Id);
        }
        public IEnumerable<int> FavoriteByIds {
            get => _favoritedBy.Select(user => user.Id);
        }

        public PamelloPlaylist(IServiceProvider services,
            DatabasePlaylist databasePlaylist
        ) : base(databasePlaylist, services) {
            _name = databasePlaylist.Name;
            _isProtected = databasePlaylist.IsProtected;
        }

        protected override void InitBase() {
            if (DatabaseEntity is null) return;

            _owner = _users.GetRequired(DatabaseEntity.Owner.Id);
            
            _favoritedBy = DatabaseEntity.FavoriteBy.Select(e => _users.GetRequired(e.Id)).ToList();
            var orderedEntries = DatabaseEntity.Entries.Where(entry => entry.PlaylistId == Id).OrderBy(entry => entry.Order);
            _songs = orderedEntries.Select(entry => base._songs.Get(entry.SongId)).OfType<PamelloSong>().ToList();
        }

        public PamelloSong? AddSong(PamelloSong song, int? position = null, bool fromInside = false) {
            if (position is null) _songs.Add(song);
            else _songs.Insert(position.Value, song);
            
            Save();

            if (!fromInside) song.AddToPlaylist(this, fromInside: true);
            _events.Broadcast(new PlaylistSongsUpdated() {
                PlaylistId = Id,
                SongsIds = SongsIds,
            });
            
            return song;
        }

        public void AddList(IReadOnlyList<PamelloSong> list, int? position = null) {
            if (list.Count == 0) return;
            
            position ??= _songs.Count;
            
            foreach (var song in list.Reverse()) {
                _songs.Insert(position.Value, song);
                song.AddToPlaylist(this, fromInside: true);
            }

            Save();

            _events.Broadcast(new PlaylistSongsUpdated() {
                PlaylistId = Id,
                SongsIds = SongsIds,
            });
        }

        public int RemoveSong(PamelloSong song, bool fromInside = false) {
            if (!_songs.Contains(song)) return 0;

            var removedCount = _songs.RemoveAll(s => s == song);
            if (removedCount == 0) return 0;
            
            Save();

            if (!fromInside) song.RemoveFromPlaylist(this, true);
            _events.Broadcast(new PlaylistSongsUpdated() {
                PlaylistId = Id,
                SongsIds = SongsIds,
            });
            
            return removedCount;
        }

        public PamelloSong? MoveSong(int fromPosition, int toPosition) {
			if (_songs.Count < 2) return null;

			fromPosition = PamelloQueue.NormalizePosition(fromPosition, _songs.Count);
			toPosition = PamelloQueue.NormalizePosition(toPosition, _songs.Count, true);

            var song = _songs[fromPosition];
            
			if (fromPosition == toPosition) return song;

            _songs.RemoveAt(fromPosition);
			if (fromPosition < toPosition) toPosition--;
            _songs.Insert(toPosition, song);
            
            Save();
            _events.Broadcast(new PlaylistSongsUpdated() {
                PlaylistId = Id,
                SongsIds = SongsIds,
            });

            return song;
        }
        
        public PamelloSong? RemoveAt(int position) {
            var song = _songs.ElementAtOrDefault(position);
            if (song is null) return null;

            _songs.RemoveAt(position);
            Save();

            if (!_songs.Contains(song)) {
                song.RemoveFromPlaylist(this, true);
            }
            
            _events.Broadcast(new PlaylistSongsUpdated() {
                PlaylistId = Id,
                SongsIds = SongsIds,
            });
            
            return song;
        }

        public void MakeFavorited(PamelloUser user) {
            if (_favoritedBy.Contains(user)) return;

            _favoritedBy.Add(user);
            Save();

            user.AddFavoritePlaylist(this);
            _events.Broadcast(new PlaylistFavoriteByIdsUpdated() {
                PlaylistId = Id,
                FavoriteByIds = FavoriteByIds
            });
        }
        public void UnmakeFavorited(PamelloUser user) {
            if (!_favoritedBy.Contains(user)) return;

            _favoritedBy.Remove(user);
            Save();

            user.RemoveFavoritePlaylist(this);
            _events.Broadcast(new PlaylistFavoriteByIdsUpdated() {
                PlaylistId = Id,
                FavoriteByIds = FavoriteByIds
            });
        }

        public override IPamelloDTO GetDTO() {
            return new PamelloPlaylistDTO() {
                Id = Id,
                Name = Name,
                AddedById = OwnedBy.Id,
                IsProtected = IsProtected,

                FavoriteByIds = FavoritedBy.Select(user => user.Id),
                SongsIds = Songs.Select(song => song.Id),
            };
        }

        public override DatabasePlaylist GetDatabaseEntity(DatabaseContext? db = null) {
            db ??= GetDatabase();

            var dbPlaylist = db.Playlists
                .Where(databasePlaylist => databasePlaylist.Id == Id)
                .Include(databasePlaylist => databasePlaylist.Entries)
                .Include(databasePlaylist => databasePlaylist.FavoriteBy)
                .AsSplitQuery()
                .FirstOrDefault();
            if (dbPlaylist is null) throw new PamelloDatabaseSaveException();

            dbPlaylist.Name = Name;
            dbPlaylist.IsProtected = IsProtected;

            var difference = Songs.Count - dbPlaylist.Entries.Count;
            if (difference > 0) {
                for (var i = 0; i < difference; i++) {
                    dbPlaylist.Entries.Add(new DatabasePlaylistEntry {
                        PlaylistId = Id
                    });
                }
            }
            else if (difference < 0) {
                for (var i = 0; i > difference; i--) {
                    dbPlaylist.Entries.RemoveAt(dbPlaylist.Entries.Count - 1);
                }
            }

            if (Songs.Count != dbPlaylist.Entries.Count) throw new PamelloDatabaseSaveException();

            var orderCount = 0;
            foreach (var song in Songs) {
                dbPlaylist.Entries[orderCount].SongId = song.Id;
                dbPlaylist.Entries[orderCount].Order = ++orderCount;
            }
            
            return dbPlaylist;
        }
    }
}
