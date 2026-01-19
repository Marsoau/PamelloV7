using PamelloV7.Core.Attributes;
using PamelloV7.Core.Audio;
using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Core.Entities;

[ValueEntity("episodes")]
public interface IPamelloEpisode : IPamelloDatabaseEntity
{
    public AudioTime Start { get; set; }
    public bool AutoSkip { get; set; }
    public IPamelloSong Song { get; }
}
