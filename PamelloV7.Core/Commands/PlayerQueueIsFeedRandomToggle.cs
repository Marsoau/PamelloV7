using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueIsFeedRandomToggle : PamelloCommand
{
    public bool Execute() {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.IsFeedRandom = !ScopeUser.RequiredSelectedPlayer.RequiredQueue.IsFeedRandom;
    }
}

