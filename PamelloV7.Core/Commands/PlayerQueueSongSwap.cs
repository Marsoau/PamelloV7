using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueSongSwap : PamelloCommand
{
    public void Execute(string inPosition, string withPosition) {
        ScopeUser.RequiredSelectedPlayer.RequiredQueue.SwapSongs(inPosition, withPosition);
    }
}

