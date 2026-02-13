using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueIsRandomToggle : PamelloCommand
{
    public bool Execute() {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.IsRandom = !ScopeUser.RequiredSelectedPlayer.RequiredQueue.IsRandom;
    }
}

