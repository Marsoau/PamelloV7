using Microsoft.EntityFrameworkCore;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model;

namespace PamelloV7.Server.Repositories
{
    public class PamelloPlaylistRepository : PamelloRepository<PamelloPlaylist, DatabasePlaylist>
    {
        public PamelloPlaylistRepository(IServiceProvider services) : base(services) {

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
