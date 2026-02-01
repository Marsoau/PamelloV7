using PamelloV7.Core.AudioOld;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories.Base;

namespace PamelloV7.Core.Repositories;

public interface IPamelloSpeakerRepository : IPamelloRepository<IPamelloSpeaker>
{
    public List<IPamelloPlayerOld> GetVoicePlayers(ulong vcId);
    Task<IPamelloInternetSpeaker> ConnectInternet(IPamelloPlayerOld player, string? name);
    
    public Task<T> GetByValueRequired<T>(string value, IPamelloUser? scopeUser)
        where T : class, IPamelloSpeaker;
}
