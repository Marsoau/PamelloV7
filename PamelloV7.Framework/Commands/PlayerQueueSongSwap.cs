using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueueSongSwap
{
    public void Execute(string inPosition, string withPosition) {
        ScopeUser.RequiredSelectedPlayer.RequiredQueue.SwapSongs(inPosition, withPosition, ScopeUser);
    }
}

