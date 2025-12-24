using PamelloV7.Core.Attributes;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Core.Services.Base;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Core.Repositories;

[EntityProvider("users")]
public interface IPamelloUserRepository : IPamelloDatabaseRepository<IPamelloUser>, IPamelloService, IEntityProvider
{
    [IdPoint]
    public IPamelloUser? Get(IPamelloUser scopeUser, int id);
    
    [NamePoint]
    public IPamelloUser? GetByName(IPamelloUser scopeUser, string query);
    
    [ValuePoint("all")]
    public IEnumerable<IPamelloUser> GetAll(IPamelloUser scopeUser);
    
    [ValuePoint(["current", "me"])]
    public IEnumerable<IPamelloUser> GetCurrent(IPamelloUser scopeUser);
    
    [ValuePoint("random")]
    public IEnumerable<IPamelloUser> GetRandom(IPamelloUser scopeUser);
    
    [ValuePoint("token")]
    public IEnumerable<IPamelloUser> GetByToken(IPamelloUser scopeUser, Guid token);
    
    public IPamelloUser? GetByToken(Guid token);
    public IPamelloUser? GetByDiscord(ulong discordId, bool createIfNotFound = true);
}
