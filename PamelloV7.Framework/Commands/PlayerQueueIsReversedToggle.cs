using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueIsReversedToggle : PamelloCommand
{
    public bool Execute() {
        RequiredQueue.SetIsReversed(!RequiredQueue.IsReversed, ScopeUser);
        return RequiredQueue.IsReversed;
    }
}

