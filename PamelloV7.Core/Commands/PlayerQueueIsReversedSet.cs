using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueIsReversedSet : PamelloCommand
{
    public bool Execute(bool state) {
        RequiredQueue.SetIsReversed(state, ScopeUser);
        return RequiredQueue.IsReversed;
    }
}

