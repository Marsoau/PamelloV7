using PamelloV7.Core.Attributes;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Core.Services.Base;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Core.Repositories;

[EntityProvider("songs")]
public interface IPamelloSongRepository : IPamelloDatabaseRepository<IPamelloSong>, IEntityProvider, IPamelloService
{
    [IdPoint]
    public IPamelloSong? Get(IPamelloUser scopeUser, int id);
    
    [NamePoint]
    public IPamelloSong? GetByName(IPamelloUser scopeUser, string query);
        
    [ValuePoint("current")]
    public IEnumerable<IPamelloSong> GetCurrent(IPamelloUser scopeUser);
        
    [ValuePoint("random")]
    public IEnumerable<IPamelloSong> GetRandom(IPamelloUser scopeUser);
        
    [ValuePoint("queue")]
    public IEnumerable<IPamelloSong> GetQueue(IPamelloUser scopeUser);
        
    [ValuePoint("all")]
    public IEnumerable<IPamelloSong> GetAll(
        IPamelloUser scopeUser,
        IPamelloUser? addedBy = null,
        IPamelloUser? favoriteBy = null
    );
        
    [ValuePoint("favorite")]
    public IEnumerable<IPamelloSong> GetFavorite(
        IPamelloUser scopeUser,
        IPamelloUser? by
    ) => GetAll(scopeUser, favoriteBy: by);
        
    [ValuePoint("added")]
    public IEnumerable<IPamelloSong> GetAdded(
        IPamelloUser scopeUser,
        IPamelloUser? by
    ) => GetAll(scopeUser, addedBy: by);
    
    //
    //
    //
    //
    //
    //
    //
    
    public IPamelloSong Add(string name, string coverUrl, IPamelloUser adder);

    public IEnumerable<IPamelloSong> Search(
        string querry,
        IPamelloUser scopeUser,
        IPamelloUser? addedBy = null,
        IPamelloUser? favoriteBy = null
    );
}
