using PamelloV7.Core.Commands.Base;

namespace PamelloV7.Core.Commands;

public class PlayerQueueSongMove : PamelloCommand
{
    public void Execute(string fromPosition, string toPosition) {
        ScopeUser.RequiredSelectedPlayer.RequiredQueue.MoveSong(fromPosition, toPosition);
    }
}

