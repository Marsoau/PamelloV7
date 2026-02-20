using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueIsFeedRandomToggle : PamelloCommand
{
    public bool Execute() {
        RequiredQueue.SetIsFeedRandom(!RequiredQueue.IsFeedRandom, ScopeUser);
        return RequiredQueue.IsFeedRandom;
    }
}

