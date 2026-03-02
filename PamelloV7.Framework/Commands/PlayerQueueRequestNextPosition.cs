using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueRequestNextPosition : PamelloCommand
{
    public int? Execute(string position) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.RequestNextPosition(position.Length == 0 || position == "null" ? null : position, ScopeUser);
    }
}

