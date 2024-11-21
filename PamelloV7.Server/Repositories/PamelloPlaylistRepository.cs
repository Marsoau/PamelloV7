using Microsoft.EntityFrameworkCore;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model;

namespace PamelloV7.Server.Repositories
{
    public class PamelloPlaylistRepository : PamelloDatabaseRepository<PamelloPlaylist, DatabasePlaylist>
    {
        public PamelloPlaylistRepository(IServiceProvider services) : base(services) {

        }

        public PamelloPlaylist? GetByName(string name) {
            var pamelloPlaylist = _loaded.FirstOrDefault(playlist => playlist.Name == name);
            if (pamelloPlaylist is not null) return pamelloPlaylist;

            var databasePlaylist = _nonloaded.FirstOrDefault(playlist => playlist.Name == name);
            if (databasePlaylist is null) return null;

            return Load(databasePlaylist);
        }

        public PamelloPlaylist? GetByValue(string value) {
            PamelloPlaylist? playlist = null;

            if (int.TryParse(value, out int id)) {
                playlist = Get(id);
            }

            playlist ??= GetByName(value);

            return playlist;
        }
        public List<PamelloPlaylist> Search(string querry, PamelloUser? ownedBy = null, PamelloUser? favoriteBy = null) {
            LoadAll();

            IEnumerable<PamelloPlaylist> list = _loaded;

            if (ownedBy is not null) {
                list = list.Where(playlist => playlist.OwnedBy.Id == ownedBy.Id);
            }
            if (favoriteBy is not null) {
                list = list.Where(playlist => playlist.FavoritedBy.Any(user => user.Id == favoriteBy.Id));
            }

            return Search(list, querry);
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
