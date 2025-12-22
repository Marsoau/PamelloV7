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
        return [Get(1)!];
    }

    public IEnumerable<IPamelloSong> GetRandom(IPamelloUser scopeUser) {
        return [_loaded[Random.Shared.Next(_loaded.Count)]];;
    }

    public IEnumerable<IPamelloSong> GetQueue(IPamelloUser scopeUser) {
        throw new NotImplementedException();
    }

    public IEnumerable<IPamelloSong> GetAll(IPamelloUser scopeUser, IPamelloUser? addedBy = null, IPamelloUser? favoriteBy = null) {
        throw new NotImplementedException();
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

    public IPamelloSong GetByYoutubeId(string youtubeId) {
        throw new NotImplementedException();
    }

    public IEnumerable<IPamelloSong> Search(string querry, IPamelloUser scopeUser, IPamelloUser? addedBy = null,
        IPamelloUser? favoriteBy = null) {
        throw new NotImplementedException();
    }
    
    public override void Delete(IPamelloSong song) {
        var pamelloSong = (PamelloSong)song;
        
        pamelloSong.IsSoftDeleted = true;
        
        pamelloSong.Save();
    }

    public void HardDelete(IPamelloSong song) {
        
    }
}
