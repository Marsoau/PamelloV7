using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueIsReversedToggle : PamelloCommand
{
    public bool Execute() {
        RequiredQueue.SetIsReversed(!RequiredQueue.IsReversed, ScopeUser);
        return RequiredQueue.IsReversed;
    }
}

