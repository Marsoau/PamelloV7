using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueSongRemoveAt : PamelloCommand
{
    public IPamelloSong? Execute(string position) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.RemoveSongAt(position, ScopeUser);
    }
}

