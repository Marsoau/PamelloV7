using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class EpisodeAutoSkipToggle : PamelloCommand
{
    public bool Execute(IPamelloEpisode episode) {
        return episode.SetAutoSkip(!episode.AutoSkip, ScopeUser);
    }
}
