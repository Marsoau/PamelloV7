using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueIsReversedSet : PamelloCommand
{
    public bool Execute(bool state) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.IsReversed = state;
    }
}

