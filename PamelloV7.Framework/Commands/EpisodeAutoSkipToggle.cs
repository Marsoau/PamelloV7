using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class EpisodeAutoSkipToggle
{
    public bool Execute(IPamelloEpisode episode) {
        return episode.SetAutoSkip(!episode.AutoSkip, ScopeUser);
    }
}
