using PamelloV7.Core.Audio;
using PamelloV7.Core.Dto;
using PamelloV7.Core.Dto.Entities;
using PamelloV7.Wrapper.Entities.Attributes;
using PamelloV7.Wrapper.Entities.Base;

namespace PamelloV7.Wrapper.Entities;

[RemoteEntity<PamelloEpisodeDto>("episodes", "IPamelloEpisode")]
public partial class RemoteEpisode
{
    public override string ToString() {
        return $"[{Id}] {Name} ({new AudioTime(Start)}{(AutoSkip ? " skip" : "")})";
    }
}
