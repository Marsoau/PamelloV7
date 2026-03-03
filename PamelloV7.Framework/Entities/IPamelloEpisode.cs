using PamelloV7.Core.Audio;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Framework.Audio;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.Entities;

[PamelloEntity("episodes", typeof(PamelloEpisodeDto))]
public interface IPamelloEpisode : IPamelloDatabaseEntity
{
    public AudioTime Start { get; set; }
    public bool AutoSkip { get; set; }
    public IPamelloSong Song { get; }
}
