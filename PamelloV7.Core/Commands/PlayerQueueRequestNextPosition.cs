using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueRequestNextPosition : PamelloCommand
{
    public int? Execute(string position) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.RequestNextPosition(position != "null" ? position : null);
    }
}

