using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueIsFeedRandomSet : PamelloCommand
{
    public bool Execute(bool state) {
        RequiredQueue.SetIsFeedRandom(state, ScopeUser);
        return RequiredQueue.IsFeedRandom;
    }
}

