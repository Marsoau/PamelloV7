using PamelloV7.Framework.Attributes;
using PamelloV7.Core.Audio;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class EpisodeStartSet
{
    public AudioTime Execute(IPamelloEpisode episode, AudioTime start) {
        return episode.SetStart(start, ScopeUser);
    }
}
