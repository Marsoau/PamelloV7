using PamelloV7.Core.DTO;
using PamelloV7.Core.Events;
using PamelloV7.Core.Exceptions;
using PamelloV7.DAL.Entity;

namespace PamelloV7.Server.Model
{
    public class PamelloPlaylist : PamelloEntity<DatabasePlaylist>
    {
        public override int Id {
            get => Entity.Id;
        }

        public override string Name {
            get => Entity.Name;
            set {
                if (Entity.Name == value) return;

                Entity.Name = value;

                _events.Broadcast(new PlaylistNameUpdated() {
                    PlaylistId = Id,
                    Name = Name
                });
            }
        }

        public bool IsProtected {
            get => Entity.IsProtected;
            set {
                if (Entity.IsProtected == value) return;

                Entity.IsProtected = value;

                _events.Broadcast(new PlaylistProtectionUpdated() {
                    PlaylistId = Id,
                    IsProtected = IsProtected
                });
            }
        }

        public PamelloUser OwnedBy {
            get => _users.GetRequired(Entity.Owner.Id);
        }

        public IReadOnlyList<PamelloSong> Songs {
            get => FavoriteByIds.Select(_songs.GetRequired).ToList();
        }
        public IReadOnlyList<PamelloUser> FavoriteBy {
            get => FavoriteByIds.Select(_users.GetRequired).ToList();
        }

        public IEnumerable<int> SongsIds {
            get => Entity.Songs.Select(song => song.Id);
        }
        public IEnumerable<int> FavoriteByIds {
            get => Entity.FavoritedBy.Select(user => user.Id);
        }

        public PamelloPlaylist(IServiceProvider services,
            DatabasePlaylist databasePlaylist
        ) : base(databasePlaylist, services) {

        }

        public void AddSong(PamelloSong song) {
            if (Entity.Songs.Any(databaseSong => databaseSong.Id == song.Id)) {
                throw new PamelloException("Song is already present in that playlist");
            }

            Entity.Songs.Add(song.Entity);
            Save();

            _events.Broadcast(new PlaylistSongsUpdated() {
                PlaylistId = Id,
                SongsIds = SongsIds,
            });
            _events.Broadcast(new SongPlaylistsIdsUpdated() {
                SongId = song.Id,
                PlaylistsIds = song.PlaylistsIds,
            });
        }

        public int AddList(IReadOnlyList<PamelloSong> list) {
            int count = 0;

            foreach (var song in list) {
                if (Entity.Songs.Any(databaseSong => databaseSong.Id == song.Id)) continue;

                Entity.Songs.Add(song.Entity);

                _events.Broadcast(new SongPlaylistsIdsUpdated() {
                    SongId = song.Id,
                    PlaylistsIds = song.PlaylistsIds,
                });

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
            if (!Entity.Songs.Any(databaseSong => databaseSong.Id == song.Id)) {
                throw new PamelloException("There is no that song in that playlist");
            }

            Entity.Songs.Remove(song.Entity);
            Save();

            _events.Broadcast(new PlaylistSongsUpdated() {
                PlaylistId = Id,
                SongsIds = SongsIds,
            });
            _events.Broadcast(new SongPlaylistsIdsUpdated() {
                SongId = song.Id,
                PlaylistsIds = song.PlaylistsIds,
            });
        }

        public override IPamelloDTO GetDTO() {
            return new PamelloPlaylistDTO() {
                Id = Id,
                Name = Name,
                AddedById = Entity.Owner.Id,
                IsProtected = IsProtected,

                FavoriteByIds = FavoriteBy.Select(user => user.Id),
                SongsIds = Songs.Select(song => song.Id),
            };
        }
    }
}
