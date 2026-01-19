using PamelloV7.Core.Attributes;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Core.Services.Base;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Core.Repositories;

[EntityProvider("playlists")]
public interface IPamelloPlaylistRepository : IPamelloDatabaseRepository<IPamelloPlaylist>, IEntityProvider, IPamelloService
{
    [IdPoint]
    public IPamelloPlaylist? Get(IPamelloUser scopeUser, int id);
    
    [NamePoint]
    public IPamelloPlaylist? GetByName(IPamelloUser scopeUser, string query);
    
    [ValuePoint("random")]
    public IEnumerable<IPamelloPlaylist> GetRandom(IPamelloUser scopeUser);
    
    [ValuePoint("all")]
    public IEnumerable<IPamelloPlaylist> GetAll(
        IPamelloUser scopeUser,
        IPamelloUser? owner = null,
        IPamelloUser? favoriteBy = null
    );
        
    [ValuePoint("favorite")]
    public IEnumerable<IPamelloPlaylist> GetFavorite(
        IPamelloUser scopeUser,
        IPamelloUser? by
    ) => GetAll(scopeUser, favoriteBy: by ?? scopeUser);
    
    [ValuePoint("owner")]
    public IEnumerable<IPamelloPlaylist> GetOwned(
        IPamelloUser scopeUser,
        IPamelloUser? owner
    ) => GetAll(scopeUser, owner: owner ?? scopeUser);
    
    //
    //
    //
    //
    //
    //
    //
    
    public IPamelloPlaylist Add(string name, IPamelloUser adder);
    
    public IEnumerable<IPamelloPlaylist> Search(
        string querry,
        IPamelloUser scopeUser,
        IPamelloUser? addedBy = null,
        IPamelloUser? favoriteBy = null
    );
}
