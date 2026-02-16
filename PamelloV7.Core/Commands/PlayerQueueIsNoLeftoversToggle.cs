using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueIsNoLeftoversToggle : PamelloCommand
{
    public bool Execute() {
        RequiredQueue.SetIsNoLeftovers(!RequiredQueue.IsNoLeftovers, ScopeUser);
        return RequiredQueue.IsNoLeftovers;
    }
}

