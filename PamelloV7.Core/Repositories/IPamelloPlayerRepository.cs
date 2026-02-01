using PamelloV7.Core.Attributes;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Core.Services.Base;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Core.Repositories;

[EntityProvider("players")]
public interface IPamelloPlayerRepository : IPamelloRepository<IPamelloPlayer>, IEntityProvider, IPamelloService
{
    [IdPoint]
    public IPamelloPlayer? Get(IPamelloUser scopeUser, int id);
    
    [NamePoint]
    public IPamelloPlayer? GetByName(IPamelloUser scopeUser, string query);
    
    [ValuePoint(["current", "my"])]
    public IEnumerable<IPamelloPlayer> GetCurrent(IPamelloUser scopeUser);
        
    [ValuePoint("random")]
    public IEnumerable<IPamelloPlayer> GetRandom(IPamelloUser scopeUser);
    
    [ValuePoint("all")]
    public IEnumerable<IPamelloPlayer> GetAll(
        IPamelloUser scopeUser,
        IPamelloUser? ownedBy = null
    );
    
    [ValuePoint("owned")]
    public IEnumerable<IPamelloPlayer> GetAdded(
        IPamelloUser scopeUser,
        IPamelloUser? by
    ) => GetAll(scopeUser, ownedBy: by ?? scopeUser);
    
    public IPamelloPlayer Create(string name, IPamelloUser creator);
}
