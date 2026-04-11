using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class EpisodeRename : PamelloCommand
{
    public string Execute(IPamelloEpisode episode, string newName) {
        return episode.SetName(newName, ScopeUser);
    }
}
