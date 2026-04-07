using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class PlayerQueueSongRemove : PamelloCommand
{
    public IEnumerable<IPamelloSong> Execute(IEnumerable<IPamelloSong> songs) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.RemoveSongs(songs, ScopeUser);
    }
}

