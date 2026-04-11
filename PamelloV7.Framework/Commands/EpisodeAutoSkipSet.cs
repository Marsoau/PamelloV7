using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class EpisodeAutoSkipSet : PamelloCommand
{
    public bool Execute(IPamelloEpisode episode, bool state) {
        return episode.SetAutoSkip(state, ScopeUser);
    }
}
