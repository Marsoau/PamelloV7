using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueIsFeedRandomToggle : PamelloCommand
{
    public bool Execute() {
        RequiredQueue.SetIsFeedRandom(!RequiredQueue.IsFeedRandom, ScopeUser);
        return RequiredQueue.IsFeedRandom;
    }
}

