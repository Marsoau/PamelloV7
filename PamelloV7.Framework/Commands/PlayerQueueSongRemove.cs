using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueSongRemove : PamelloCommand
{
    public IPamelloSong Execute(string position) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.RemoveSong(position, ScopeUser);
    }
}

