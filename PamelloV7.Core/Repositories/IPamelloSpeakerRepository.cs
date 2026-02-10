using PamelloV7.Core.Attributes;
using PamelloV7.Core.AudioOld;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Core.Services.Base;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Core.Repositories;

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
