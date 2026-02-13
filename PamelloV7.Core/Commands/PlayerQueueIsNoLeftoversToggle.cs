using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueIsNoLeftoversToggle : PamelloCommand
{
    public bool Execute() {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.IsNoLeftovers = !ScopeUser.RequiredSelectedPlayer.RequiredQueue.IsNoLeftovers;
    }
}

