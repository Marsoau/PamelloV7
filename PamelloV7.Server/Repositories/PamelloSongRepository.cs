using Microsoft.EntityFrameworkCore;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model;

namespace PamelloV7.Server.Repositories
{
    public class PamelloSongRepository : PamelloRepository<PamelloSong, DatabaseSong>
    {
        public PamelloSongRepository(IServiceProvider services) : base(services) {

        }

        public PamelloSong? GetByAssociacion(string ascn) {
            var databaseAssociacion = _database.Associacions.Find(ascn);
            if (databaseAssociacion is null) return null;

            return Load(databaseAssociacion.Song);
        }

        public PamelloSong? GetByYoutubeId(string youtubeId) {
            var pamelloSong = _loaded.FirstOrDefault(song => song.YoutubeId == youtubeId);
            if (pamelloSong is not null) return pamelloSong;

            var databaseSong = _nonloaded.FirstOrDefault(song => song.YoutubeId == youtubeId);
            if (databaseSong is null) return null;

            return Load(databaseSong);
        }

        public override void Delete(int id) => throw new NotImplementedException();
        public override PamelloSong Load(DatabaseSong databaseSong) {
            var pamelloSong = _loaded.FirstOrDefault(song => song.Id == databaseSong.Id);
            if (pamelloSong is not null) return pamelloSong;

            pamelloSong = new PamelloSong(_services, databaseSong);
            _loaded.Add(pamelloSong);

            return pamelloSong;
        }
        public override List<DatabaseSong> LoadDatabaseEntities() {
            return _database.Songs
                .Include(song => song.Episodes)
                .Include(song => song.Playlists)
                .Include(song => song.FavoritedBy)
                .Include(song => song.Associacions)
                .ToList();
        }
    }
}
