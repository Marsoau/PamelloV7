using PamelloV7.Core.Audio;
using PamelloV7.Framework.Audio;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Repositories.Base;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Framework.Services.PEQL;

namespace PamelloV7.Framework.Repositories;

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
    
    public IPamelloEpisode Add(AudioTime start, string name, bool autoSkip, IPamelloSong song, IPamelloUser scopeUser);
    public void DeleteAllFrom(IPamelloSong song, IPamelloUser scopeUser);
}
