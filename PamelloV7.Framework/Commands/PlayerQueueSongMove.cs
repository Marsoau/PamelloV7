using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueSongMove
{
    public void Execute(string fromPosition, string toPosition) {
        ScopeUser.RequiredSelectedPlayer.RequiredQueue.MoveSong(fromPosition, toPosition, ScopeUser);
    }
}

