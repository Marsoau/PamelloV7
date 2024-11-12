using Microsoft.EntityFrameworkCore;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model;

namespace PamelloV7.Server.Repositories
{
    public class PamelloEpisodeRepository : PamelloRepository<PamelloEpisode, DatabaseEpisode>
    {
        public PamelloEpisodeRepository(IServiceProvider services            

        ) : base(services) {

        }

        public override void Delete(int id) => throw new NotImplementedException();
        public override PamelloEpisode Load(DatabaseEpisode databaseEpisode) {
            var pamelloEpisode = _loaded.FirstOrDefault(episode => episode.Id == databaseEpisode.Id);
            if (pamelloEpisode is not null) return pamelloEpisode;

            pamelloEpisode = new PamelloEpisode(_services, databaseEpisode);
            _loaded.Add(pamelloEpisode);

            return pamelloEpisode;
        }
        public override List<DatabaseEpisode> LoadDatabaseEntities() {
            return _database.Episodes.ToList();
        }
    }
}
