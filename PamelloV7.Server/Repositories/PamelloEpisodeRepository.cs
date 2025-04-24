using Microsoft.EntityFrameworkCore;
using PamelloV7.Core.Audio;
using PamelloV7.DAL.Entity;
using PamelloV7.Server.Model;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Events;

namespace PamelloV7.Server.Repositories
{
    public class PamelloEpisodeRepository : PamelloDatabaseRepository<PamelloEpisode, DatabaseEpisode>
    {
        private PamelloSongRepository _songs;

        public PamelloEpisodeRepository(IServiceProvider services            

        ) : base(services) {

        }
        public override void InitServices() {
            _songs = _services.GetRequiredService<PamelloSongRepository>();

            base.InitServices();
        }
        public PamelloEpisode Create(PamelloSong song, AudioTime start, string name, bool skip) {
            var db = GetDatabase();

            var databaseEpisode = new DatabaseEpisode() {
                Name = name,
                Start = start.TotalSeconds,
                Skip = false,
                Song = song.Entity
            };

            db.Episodes.Add(databaseEpisode);
            db.SaveChangesAsync();

            _events.Broadcast(new EpisodeCreated() { 
                EpisodeId = databaseEpisode.Id,
            });
            _events.Broadcast(new SongEpisodesIdsUpdated() { 
                SongId = song.Id,
                EpisodesIds = song.EpisodesIds,
            });

            return Load(databaseEpisode);
        }

        public override void Delete(int id) {
            var episode = GetRequired(id);

            _loaded.Remove(episode);

            var db = GetDatabase();

            db.Episodes.Remove(episode.Entity);
            db.SaveChanges();

            _events.Broadcast(new EpisodeDeleted() { 
                 EpisodeId = episode.Id,
            });
            _events.Broadcast(new SongEpisodesIdsUpdated() { 
                SongId = episode.Song.Id,
                EpisodesIds = episode.Song.EpisodesIds,
            });
        }
        public void DeleteAllFrom(PamelloSong song) {
            var db = GetDatabase();

            var deletionList = db.Episodes.Where(databaseEpisode => databaseEpisode.Song.Id == song.Id);

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
        public override List<DatabaseEpisode> ProvideEntities() {
            return GetDatabase().Episodes.ToList();
        }

        public override async Task<PamelloEpisode?> GetByValue(string value, PamelloUser? scopeUser) {
            PamelloEpisode? episode = null;

            if (value == "current") {
                episode = scopeUser?.SelectedPlayer?.Queue.Current?.GetCurrentEpisode();
            }
            else if (value == "random") {
                episode = GetRandom(scopeUser?.SelectedPlayer?.Queue.Current?.Song);
            }
            else if (int.TryParse(value, out var id)) {
                episode = Get(id);
            }
            else {
                var values = value.Split(':');
                if (values.Length != 2)
                    throw new PamelloException("invalid episode value format");
                if (!int.TryParse(values[1], out int position))
                    throw new PamelloException("cant parse episode position to int");

                var song = await _songs.GetByValue(values[0], scopeUser);
                episode = song?.Episodes.ElementAtOrDefault(position);
            }

            return episode;
        }

        public PamelloEpisode? GetRandom(PamelloSong fromSong) {
            if (fromSong is null) return null;

            var episodes = fromSong.Episodes;
            var randomPosition = Random.Shared.Next(0, episodes.Count);
            return episodes[randomPosition];
        }
    }
}
