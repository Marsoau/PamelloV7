using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueIsFeedRandomSet : PamelloCommand
{
    public bool Execute(bool state) {
        RequiredQueue.SetIsFeedRandom(state, ScopeUser);
        return RequiredQueue.IsFeedRandom;
    }
}

