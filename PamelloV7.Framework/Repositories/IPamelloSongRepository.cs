using PamelloV7.Framework.History.Records;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Platforms.Infos;
using PamelloV7.Framework.Repositories.Base;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Framework.Services.PEQL;

namespace PamelloV7.Framework.Repositories;

[EntityProvider("songs")]
public interface IPamelloSongRepository : IPamelloDatabaseRepository<IPamelloSong>, IEntityProvider, IPamelloService
{
    [IdPoint]
    public IPamelloSong? Get(IPamelloUser scopeUser, int id);
    
    [NamePoint]
    public Task<IPamelloSong?> GetByNameAsync(IPamelloUser scopeUser, string query);
    
    [PlatformKeyPoint]
    public Task<IPamelloSong?> GetByPlatformKeyAsync(IPamelloUser scopeUser, PlatformKey pk, bool allowCreation = false);
        
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
    );
        
    [ValuePoint("added")]
    public IEnumerable<IPamelloSong> GetAdded(
        IPamelloUser scopeUser,
        IPamelloUser? by
    ) => GetAll(scopeUser, addedBy: by ?? scopeUser);

    [ValuePoint("playlist")]
    public IEnumerable<IPamelloSong> GetFromPlaylist(
        IPamelloUser scopeUser,
        IPamelloPlaylist? playlist
    );
    
    [ValuePoint("test")]
    public IEnumerable<IPamelloSong> TestPoint(
        IPamelloUser scopeUser,
        int value
    );
    
    //
    //
    //
    //
    //
    //
    //
    
    public IPamelloSong Add(ISongInfo info, IPamelloUser adder);
    
    public IEnumerable<IPamelloSong> Search(
        string querry,
        IPamelloUser scopeUser,
        IPamelloUser? addedBy = null,
        IPamelloUser? favoriteBy = null
    );
}
