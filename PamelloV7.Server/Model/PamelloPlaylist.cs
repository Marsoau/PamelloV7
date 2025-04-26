using Microsoft.EntityFrameworkCore;
using PamelloV7.Core;
using PamelloV7.Core.DTO;
using PamelloV7.Core.Events;
using PamelloV7.Core.Exceptions;
using PamelloV7.DAL;
using PamelloV7.DAL.Entity;

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

        protected override void InitSet() {
            if (DatabaseEntity is null) return;

            _owner = _users.GetRequired(DatabaseEntity.Owner.Id);
            _songs = DatabaseEntity.Songs.Select(e => base._songs.GetRequired(e.Id)).ToList();
            _favoritedBy = DatabaseEntity.FavoritedBy.Select(e => _users.GetRequired(e.Id)).ToList();
        }

        public void AddSong(PamelloSong song) {
            if (_songs.Contains(song)) return;

            _songs.Add(song);
            Save();

            song.AddToPlaylist(this);
            _events.Broadcast(new PlaylistSongsUpdated() {
                PlaylistId = Id,
                SongsIds = SongsIds,
            });
        }

        public int AddList(IReadOnlyList<PamelloSong> list) {
            int count = 0;

            foreach (var song in list) {
                if (_songs.Contains(song)) continue;

                _songs.Add(song);
                song.AddToPlaylist(this);

                count++;
            }

            if (count > 0) {
                Save();
            }

            _events.Broadcast(new PlaylistSongsUpdated() {
                PlaylistId = Id,
                SongsIds = SongsIds,
            });

            return count;
        }

        public void RemoveSong(PamelloSong song) {
            if (!_songs.Contains(song)) return;

            _songs.Remove(song);
            Save();

            song.RemoveFromPlaylist(this);
            _events.Broadcast(new PlaylistSongsUpdated() {
                PlaylistId = Id,
                SongsIds = SongsIds,
            });
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
                .Include(databasePlaylist => databasePlaylist.Songs)
                .Include(databasePlaylist => databasePlaylist.FavoritedBy)
                .AsSplitQuery()
                .FirstOrDefault();
            if (dbPlaylist is null) throw new PamelloDatabaseSaveException();

            dbPlaylist.Name = Name;
            dbPlaylist.IsProtected = IsProtected;

            var dbSongsIds = dbPlaylist.Songs.Select(song => song.Id);
            var dbFavoriteByIds = dbPlaylist.FavoritedBy.Select(song => song.Id);

            var addedSongsDifference = DifferenceResult<int>.From(
                dbSongsIds, 
                SongsIds,
                true
            );
            var addedPlaylistsDifference = DifferenceResult<int>.From(
                dbFavoriteByIds, 
                FavoriteByIds,
                true
            );

            addedSongsDifference.ExcludeMoved();
            addedPlaylistsDifference.ExcludeMoved();

            addedSongsDifference.Apply(dbPlaylist.Songs, (songId) => {
                return db.Songs.Find(songId)!;
            });
            addedPlaylistsDifference.Apply(dbPlaylist.FavoritedBy, (playlistId) => {
                return db.Users.Find(playlistId)!;
            });

            return dbPlaylist;
        }
    }
}
