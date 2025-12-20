using PamelloV7.Core.Audio;
using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Repositories;

public interface IPamelloEpisodeRepository : IPamelloDatabaseRepository<IPamelloEpisode>, IPamelloService
{
    public IPamelloEpisode Add(AudioTime start, string name, bool autoSkip, IPamelloSong song);
    public void DeleteAllFrom(IPamelloSong song);
}
