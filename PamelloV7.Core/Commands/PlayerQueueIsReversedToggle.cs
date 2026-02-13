using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueIsReversedToggle : PamelloCommand
{
    public bool Execute() {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.IsReversed = !ScopeUser.RequiredSelectedPlayer.RequiredQueue.IsReversed;
    }
}

