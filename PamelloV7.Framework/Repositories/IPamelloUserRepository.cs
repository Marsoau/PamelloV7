using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Platforms.Infos;
using PamelloV7.Framework.Repositories.Base;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Framework.Services.PEQL;

namespace PamelloV7.Framework.Repositories;

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
    
    public Task<IPamelloUser?> GetByPlatformKey(PlatformKey pk, bool? allowCreation = null);
    
    public IPamelloUser Add(IUserInfo info);
}
