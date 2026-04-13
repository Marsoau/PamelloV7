using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueRequestNextPosition
{
    public int? Execute(string position) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.RequestNextPosition(position.Length == 0 || position == "null" ? null : position, ScopeUser);
    }
}

