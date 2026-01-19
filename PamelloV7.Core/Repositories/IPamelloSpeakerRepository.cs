using PamelloV7.Core.Audio;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Repositories.Base;

namespace PamelloV7.Core.Repositories;

public interface IPamelloSpeakerRepository : IPamelloRepository<IPamelloSpeaker>
{
    public List<IPamelloPlayer> GetVoicePlayers(ulong vcId);
    Task<IPamelloInternetSpeaker> ConnectInternet(IPamelloPlayer player, string? name);
    
    public Task<T> GetByValueRequired<T>(string value, IPamelloUser? scopeUser)
        where T : class, IPamelloSpeaker;
}
