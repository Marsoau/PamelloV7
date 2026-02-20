using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueSongSwap : PamelloCommand
{
    public void Execute(string inPosition, string withPosition) {
        ScopeUser.RequiredSelectedPlayer.RequiredQueue.SwapSongs(inPosition, withPosition, ScopeUser);
    }
}

