using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueIsNoLeftoversToggle : PamelloCommand
{
    public bool Execute() {
        RequiredQueue.SetIsNoLeftovers(!RequiredQueue.IsNoLeftovers, ScopeUser);
        return RequiredQueue.IsNoLeftovers;
    }
}

