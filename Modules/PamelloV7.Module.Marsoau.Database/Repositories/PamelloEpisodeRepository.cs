using PamelloV7.Core.Audio;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Data.Entities;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.History.Records;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.Base.Repositories.Database.Base;
using PamelloV7.Module.Marsoau.Database.Entities;
using PamelloV7.Module.Marsoau.Database.Events.RestorePacks;

namespace PamelloV7.Module.Marsoau.Database.Repositories;

public class PamelloEpisodeRepository : PamelloDatabaseRepository<IPamelloEpisode, DatabaseEpisode>, IPamelloEpisodeRepository
{
    public override string CollectionName => "episodes";
    
    public PamelloEpisodeRepository(IServiceProvider services) : base(services) {
    }

    protected override IPamelloEpisode CreatePamelloEntity(DatabaseEpisode databaseEntity) {
        return new PamelloEpisode(databaseEntity, _services);
    }

    public IPamelloEpisode? Get(IPamelloUser scopeUser, int id) {
        return Get(id);
    }

    public IPamelloEpisode? GetByName(IPamelloUser scopeUser, string query) {
        throw new NotImplementedException();
    }

    public IEnumerable<IPamelloEpisode> GetAll(IPamelloUser scopeUser) {
        return _loaded.AsReadOnly();
    }

    public IEnumerable<IPamelloEpisode> GetCurrent(IPamelloUser scopeUser) {
        var episode = scopeUser?.SelectedPlayer?.Queue?.CurrentEpisode;
        return episode is not null ? [episode] : [];
    }

    public IEnumerable<IPamelloEpisode> GetRandom(IPamelloUser scopeUser) {
        var episode = _loaded.ElementAtOrDefault(Random.Shared.Next(_loaded.Count));
        return episode is not null ? [episode] : [];
    }

    public IEnumerable<IPamelloEpisode> GetFromSong(IPamelloUser scopeUser, IPamelloSong song) {
        return song.Episodes;
    }

    public IPamelloEpisode Add(AudioTime start, string name, bool autoSkip, IPamelloSong song, IPamelloUser scopeUser) {
        var songEpisodes = ((PamelloSong)song)._songEpisodes;
        if (songEpisodes.Any(e => e.Start.TotalSeconds == start.TotalSeconds)) throw new PamelloException("Episode with this start time already exists");
        
        var databaseEpisode = new DatabaseEpisode() {
            StartSeconds = start.TotalSeconds,
            Name = name,
            AutoSkip = autoSkip,
            SongId = song.Id,
        };
        
        GetCollection().Add(databaseEpisode);
        
        var episode = Load(databaseEpisode);
        
        int i;
        for (i = 0; i < songEpisodes.Count && songEpisodes[i].Start.TotalSeconds < start.TotalSeconds; i++);
        songEpisodes.Insert(i, episode);
        
        return episode;
    }

    public override IHistoryRecord Delete(IPamelloEpisode episode, IPamelloUser? scopeUser) {
        var collection = GetCollection();
        
        var pamelloSong = (PamelloSong)episode.Song;
        var databaseEpisode = collection.Get(episode.Id)!;
        
        pamelloSong._songEpisodes.Remove(episode);
        
        _loaded.Remove(episode);
        collection.Delete(episode.Id);
        
        pamelloSong.Save();

        var record = _events.Invoke(scopeUser, new EpisodeDeleted() {
            RevertPack = new EpisodeDeletionRevertPack() {
                DatabaseEpisode = databaseEpisode
            },
            Episode = episode
        })!;
        
        return record;
    }
    
    public void Restore(IPamelloUser scopeUser, DatabaseEpisode databaseEpisode) {
        var collection = GetCollection();

        if (collection.Get(databaseEpisode.Id) is not null) {
            throw new PamelloException("Episode already exists in the database");
        }
        
        collection.Add(databaseEpisode);
        
        var episode = GetRequired(databaseEpisode.Id);

        _events.Invoke(scopeUser, new EpisodeRestored() {
            RevertPack = new EpisodeRestoreRevertPack(),
            Episode = episode
        });
    }

    public void DeleteAllFrom(IPamelloSong song, IPamelloUser scopeUser) {
        var pamelloSong = (PamelloSong)song;
        
        pamelloSong._songEpisodes.Clear();
        
        GetCollection().DeleteMany(e => e.SongId == song.Id);
    }
}
