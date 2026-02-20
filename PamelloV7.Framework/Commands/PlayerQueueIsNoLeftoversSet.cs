using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueIsNoLeftoversSet : PamelloCommand
{
    public bool Execute(bool state) {
        RequiredQueue.SetIsNoLeftovers(state, ScopeUser);
        return RequiredQueue.IsNoLeftovers;
    }
}

