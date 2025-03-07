using Microsoft.EntityFrameworkCore;
using PamelloV7.Core.Exceptions;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model;

namespace PamelloV7.Server.Repositories
{
    public class PamelloPlaylistRepository : PamelloDatabaseRepository<PamelloPlaylist, DatabasePlaylist>
    {
        public PamelloPlaylistRepository(IServiceProvider services) : base(services) {

        }
        public override void InitServices() {
            base.InitServices();
        }

        public PamelloPlaylist Create(string name, PamelloUser user) {
            if (name.Length == 0) throw new PamelloException("Playlist name cant be empty");

            var databasePlaylist = new DatabasePlaylist() {
                Name = name,
                Songs = [],
                Owner = user.Entity,
                IsProtected = false,
                FavoritedBy = [],
            };

            _database.Playlists.Add(databasePlaylist);
            _database.SaveChanges();

            return Load(databasePlaylist);
        }

        public PamelloPlaylist GetByNameRequired(string name)
            => GetByName(name) ?? throw new PamelloException($"Cant find playlist by name \"{name}\"");

        public PamelloPlaylist? GetByName(string name) {
            var pamelloPlaylist = _loaded.FirstOrDefault(playlist => playlist.Name == name);
            if (pamelloPlaylist is not null) return pamelloPlaylist;

            var databasePlaylist = _nonloaded.FirstOrDefault(playlist => playlist.Name == name);
            if (databasePlaylist is null) return null;

            return Load(databasePlaylist);
        }

        public override Task<PamelloPlaylist?> GetByValue(string value, PamelloUser? scopeUser = null) {
            PamelloPlaylist? playlist = null;

            if (int.TryParse(value, out int id)) {
                playlist = Get(id);
            }

            playlist ??= GetByName(value);

            return Task.FromResult(playlist);
        }

        public async Task<IEnumerable<PamelloPlaylist>> Search(string querry, PamelloUser? ownedBy = null, PamelloUser? favoriteBy = null, PamelloUser? scopeUser = null) {
            LoadAll();

            IEnumerable<PamelloPlaylist> list = _loaded;

            if (ownedBy is not null) {
                list = list.Where(playlist => playlist.OwnedBy.Id == ownedBy.Id);
            }
            if (favoriteBy is not null) {
                list = list.Where(playlist => playlist.FavoriteBy.Any(user => user.Id == favoriteBy.Id));
            }

            return await Search(list, querry, scopeUser);
        }

        public override PamelloPlaylist Load(DatabasePlaylist databasePlaylist) {
            var pamelloPlaylist = _loaded.FirstOrDefault(playlist => playlist.Id == databasePlaylist.Id);
            if (pamelloPlaylist is not null) return pamelloPlaylist;

            pamelloPlaylist = new PamelloPlaylist(_services, databasePlaylist);
            _loaded.Add(pamelloPlaylist);

            return pamelloPlaylist;
        }
        public override List<DatabasePlaylist> LoadDatabaseEntities() {
            return _database.Playlists
                .Include(playlist => playlist.Songs)
                .Include(playlist => playlist.FavoritedBy)
                .ToList();
        }
        public override void Delete(int id) => throw new NotImplementedException();
    }
}
