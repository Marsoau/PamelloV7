using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class PlayerQueueSongRemove : PamelloCommand
{
    public IPamelloSong Execute(string position) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.RemoveSong(position);
    }
}

