using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class PlayerQueueSongAdd : PamelloCommand
{
    public IEnumerable<IPamelloSong> Execute(IEnumerable<IPamelloSong> songs) {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.AddSongs(songs, ScopeUser);
    }
}
