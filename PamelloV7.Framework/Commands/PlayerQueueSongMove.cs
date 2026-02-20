using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueSongMove : PamelloCommand
{
    public void Execute(string fromPosition, string toPosition) {
        ScopeUser.RequiredSelectedPlayer.RequiredQueue.MoveSong(fromPosition, toPosition, ScopeUser);
    }
}

