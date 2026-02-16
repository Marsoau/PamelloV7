using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueIsNoLeftoversSet : PamelloCommand
{
    public bool Execute(bool state) {
        RequiredQueue.SetIsNoLeftovers(state, ScopeUser);
        return RequiredQueue.IsNoLeftovers;
    }
}

