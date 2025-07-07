using Microsoft.EntityFrameworkCore;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Events;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model;

namespace PamelloV7.Server.Repositories.Database
{
    public class PamelloEpisodeRepository : PamelloDatabaseRepository<IPamelloEpisode, DatabaseEpisode>, IPamelloEpisodeRepository
    {
        private IPamelloSongRepository _songs;

        public PamelloEpisodeRepository(IServiceProvider services            

        ) : base(services) {

        }
        public override void InitServices() {
            base.InitServices();

            _songs = _services.GetRequiredService<IPamelloSongRepository>();
        }
        public IPamelloEpisode Create(IPamelloSong song, AudioTime start, string name, bool skip) {
            return song.AddEpisode(start, name, skip);
        }

        public override void Delete(IPamelloEpisode episode) {
            if (!_loaded.Remove(episode)) return;

            var db = GetDatabase();

            var dbEpisode = db.Episodes.Find(episode.Id);
            
            if (dbEpisode is not null) {
                db.Episodes.Remove(dbEpisode);
                db.SaveChanges();

                var song = _songs.Get(dbEpisode.Song.Id);
                if (song is not null) {
                    song.RemoveEpisode(episode);
                }
            }

            _events.Broadcast(new EpisodeDeleted() { 
                 EpisodeId = episode.Id,
            });
        }
        public void DeleteAllFrom(IPamelloSong song) {
            song.RemoveAllEpisodes();
        }
        protected override IPamelloEpisode LoadBase(DatabaseEpisode databaseEpisode) {
            var pamelloEpisode = _loaded.FirstOrDefault(episode => episode.Id == databaseEpisode.Id);
            if (pamelloEpisode is not null) return pamelloEpisode;

            pamelloEpisode = new PamelloEpisode(_services, databaseEpisode);
            _loaded.Add(pamelloEpisode);

            return pamelloEpisode;
        }
        public override List<DatabaseEpisode> ProvideEntities() {
            return GetDatabase().Episodes
                .AsNoTracking()
                .Include(episode => episode.Song)
                .ToList();
        }

        public override IPamelloEpisode? GetByValueSync(string value, IPamelloUser? scopeUser) {
            IPamelloEpisode? episode = null;

            if (value == "current") {
                episode = scopeUser?.SelectedPlayer?.Queue.Audio?.GetCurrentEpisode();
            }
            else if (value == "random") {
                episode = GetRandom(scopeUser?.SelectedPlayer?.Queue.Audio?.Song);
            }
            else if (int.TryParse(value, out var id)) {
                episode = Get(id);
            }
            else {
                episode = GetFromSplitValue(value, scopeUser, _songs, (song, secondValue) => {
                    if (!int.TryParse(secondValue, out var position))
                        return null;
                    
                    return song?.Episodes.ElementAtOrDefault(position);
                }).Result;
            }

            return episode;
        }

        public IPamelloEpisode? GetRandom(IPamelloSong fromSong) {
            if (fromSong is null) return null;

            var episodes = fromSong.Episodes;
            var randomPosition = Random.Shared.Next(0, episodes.Count);
            return episodes.ElementAtOrDefault(randomPosition);
        }

        public override void Dispose() {
            Console.WriteLine("Disposing episodes");
        }
    }
}
