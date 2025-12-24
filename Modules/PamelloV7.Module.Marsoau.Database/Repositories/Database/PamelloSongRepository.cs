using PamelloV7.Core.Data.Entities;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Base.Entities;
using PamelloV7.Module.Marsoau.Base.Repositories.Database.Base;
using PamelloV7.Server.Entities;

namespace PamelloV7.Module.Marsoau.Base.Repositories.Database;

public class PamelloSongRepository : PamelloDatabaseRepository<IPamelloSong, DatabaseSong>, IPamelloSongRepository
{
    public PamelloSongRepository(IServiceProvider services) : base(services) {
    }

    public override string CollectionName => "songs";
    protected override IPamelloSong CreatePamelloEntity(DatabaseSong databaseEntity) {
        return new PamelloSong(databaseEntity, _services);
    }

    public IPamelloSong? Get(IPamelloUser scopeUser, int id) {
        return Get(id);
    }

    public IPamelloSong? GetByName(IPamelloUser scopeUser, string query) {
        throw new NotImplementedException();
    }

    public IEnumerable<IPamelloSong> GetCurrent(IPamelloUser scopeUser) {
        var queue = scopeUser.SelectedPlayer?.Queue;
        if (queue is null) return [];

        var current = queue.Entries[queue.Position].Song;

        return [current];
    }

    public IEnumerable<IPamelloSong> GetRandom(IPamelloUser scopeUser) {
        var song = _loaded.ElementAtOrDefault(Random.Shared.Next(_loaded.Count));
        return song is not null ? [song] : [];
    }

    public IEnumerable<IPamelloSong> GetQueue(IPamelloUser scopeUser) {
        var queue = scopeUser.SelectedPlayer?.Queue;
        return queue is null ? [] : queue.Entries.Select(entry => entry.Song);
    }

    public IEnumerable<IPamelloSong> GetAll(IPamelloUser scopeUser, IPamelloUser? addedBy = null, IPamelloUser? favoriteBy = null) {
        IEnumerable<IPamelloSong> results = _loaded.AsReadOnly();
        
        if (addedBy is not null) results = results.Where(s => s.AddedBy == addedBy);
        if (favoriteBy is not null) results = results.Where(s => s.FavoriteBy.Contains(favoriteBy));
        
        return results;
    }

    public IEnumerable<IPamelloSong> GetFromPlaylist(IPamelloUser scopeUser, IPamelloPlaylist? playlist) {
        return playlist?.Songs ?? [];
    }

    public IEnumerable<IPamelloSong> TestPoint(IPamelloUser scopeUser, int value) {
        Console.WriteLine($"TTEEST PINT: {value}");
        return [];
    }

    public IPamelloSong Add(string name, string coverUrl, IPamelloUser adder) {
        var databaseSong = new DatabaseSong() {
            Name = name,
            CoverUrl = coverUrl,
            Associations = [],
            Sources = [],
            AddedBy = adder.Id,
            AddedAt = DateTime.Now,
        };
        
        GetCollection().Add(databaseSong);
        
        return Load(databaseSong);
    }

    public IEnumerable<IPamelloSong> Search(string querry, IPamelloUser scopeUser, IPamelloUser? addedBy = null,
        IPamelloUser? favoriteBy = null) {
        throw new NotImplementedException("because author is gay");
    }
    
    public override void Delete(IPamelloSong song) {
        var pamelloSong = (PamelloSong)song;
        
        pamelloSong.IsSoftDeleted = true;
        
        pamelloSong.Save();
    }

    public void HardDelete(IPamelloSong song) {
        
    }
}
