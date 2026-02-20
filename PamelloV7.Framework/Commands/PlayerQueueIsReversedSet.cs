using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueIsReversedSet : PamelloCommand
{
    public bool Execute(bool state) {
        RequiredQueue.SetIsReversed(state, ScopeUser);
        return RequiredQueue.IsReversed;
    }
}

