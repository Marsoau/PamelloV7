using PamelloV7.Core.Audio.Time;
using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Base.Repositories.Database.Base;
using PamelloV7.Module.Marsoau.Database.Entities;

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

    public IPamelloEpisode Add(AudioTime start, string name, bool autoSkip, IPamelloSong song) {
        var songEpisodes = ((PamelloSong)song)._songEpisodes;
        if (songEpisodes.Any(e => e.Start.TotalSeconds == start.TotalSeconds)) return null;
        
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

    public override void Delete(IPamelloUser scopeUser, IPamelloEpisode episode) {
        var pamelloSong = (PamelloSong)episode.Song;
        
        pamelloSong._songEpisodes.Remove(episode);
        
        _loaded.Remove(episode);
        GetCollection().Delete(episode.Id);
        
        pamelloSong.Save();
    }

    public void DeleteAllFrom(IPamelloSong song) {
        var pamelloSong = (PamelloSong)song;
        
        pamelloSong._songEpisodes.Clear();
        
        GetCollection().DeleteMany(e => e.SongId == song.Id);
    }
}
