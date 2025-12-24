using PamelloV7.Core.Attributes;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Core.Services.Base;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Core.Repositories;

[EntityProvider("episodes")]
public interface IPamelloEpisodeRepository : IPamelloDatabaseRepository<IPamelloEpisode>, IEntityProvider, IPamelloService
{
    [IdPoint]
    public IPamelloEpisode? Get(IPamelloUser scopeUser, int id);
    
    [NamePoint]
    public IPamelloEpisode? GetByName(IPamelloUser scopeUser, string query);
    
    [ValuePoint("all")]
    public IEnumerable<IPamelloEpisode> GetAll(IPamelloUser scopeUser);
    
    [ValuePoint("current")]
    public IEnumerable<IPamelloEpisode> GetCurrent(IPamelloUser scopeUser);
    
    [ValuePoint("random")]
    public IEnumerable<IPamelloEpisode> GetRandom(IPamelloUser scopeUser);
    
    [ValuePoint("song")]
    public IEnumerable<IPamelloEpisode> GetFromSong(IPamelloUser scopeUser, IPamelloSong song);
    
    public IPamelloEpisode Add(AudioTime start, string name, bool autoSkip, IPamelloSong song);
    public void DeleteAllFrom(IPamelloSong song);
}
