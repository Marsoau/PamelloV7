using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Repositories.Base;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Framework.Services.PEQL;

namespace PamelloV7.Framework.Repositories;

[EntityProvider("speakers")]
public interface IPamelloSpeakerRepository : IPamelloRepository<IPamelloSpeaker>, IEntityProvider, IPamelloService
{
    public int NextId { get; }
    
    [IdPoint]
    public IPamelloSpeaker? Get(IPamelloUser scopeUser, int id);
    
    [NamePoint]
    public IPamelloSpeaker? GetByName(IPamelloUser scopeUser, string query);
    
    [ValuePoint("current")]
    public IEnumerable<IPamelloSpeaker> GetCurrent(IPamelloUser scopeUser);
    
    [ValuePoint("random")]
    public IEnumerable<IPamelloSpeaker> GetRandom(IPamelloUser scopeUser);
    
    [ValuePoint("player")]
    public IEnumerable<IPamelloSpeaker> GetFromPlayer(IPamelloUser scopeUser, IPamelloPlayer player);
    
    [ValuePoint("all")]
    public IEnumerable<IPamelloSpeaker> GetAll(IPamelloUser scopeUser);
    
    public TPamelloSpeaker Add<TPamelloSpeaker>(TPamelloSpeaker speaker)
        where TPamelloSpeaker : IPamelloSpeaker;
}
