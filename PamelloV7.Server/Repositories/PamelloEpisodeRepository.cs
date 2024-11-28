using Microsoft.EntityFrameworkCore;
using PamelloV7.Core.Audio;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model;

namespace PamelloV7.Server.Repositories
{
    public class PamelloEpisodeRepository : PamelloDatabaseRepository<PamelloEpisode, DatabaseEpisode>
    {
        public PamelloEpisodeRepository(IServiceProvider services            

        ) : base(services) {

        }
        public override void InitServices() {

        }
        public PamelloEpisode Create(PamelloSong song, AudioTime start, string name, bool skip) {
            var databaseEpisode = new DatabaseEpisode() {
                Name = name,
                Start = start.TotalSeconds,
                Skip = false,
                Song = song.Entity
            };

            _database.Episodes.Add(databaseEpisode);
            _database.SaveChanges();

            return Load(databaseEpisode);
        }

        public override void Delete(int id) {
            var episode = GetRequired(id);

            _loaded.Remove(episode);
            _database.Episodes.Remove(episode.Entity);
            _database.SaveChanges();
        }
        public void DeleteAllFrom(PamelloSong song) {
            var deletionList = _database.Episodes.Where(databaseEpisode => databaseEpisode.Song.Id == song.Id);

            foreach (var deletion in deletionList) {
                Delete(deletion.Id);
            }
        }
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
