using PamelloV7.Core.Audio;
using PamelloV7.Core.Model.Entities.Base;

namespace PamelloV7.Core.Model.Entities;

public interface IPamelloEpisode : IPamelloEntity
{
    public AudioTime Start { get; set; }
    public bool AutoSkip { get; set; }
    public IPamelloSong Song { get; }
}
