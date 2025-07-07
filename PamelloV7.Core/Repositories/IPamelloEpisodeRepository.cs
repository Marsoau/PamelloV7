using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Repositories.Base;

namespace PamelloV7.Core.Repositories;

public interface IPamelloEpisodeRepository : IPamelloRepository<IPamelloEpisode>
{
    public void DeleteAllFrom(IPamelloSong song);
}
