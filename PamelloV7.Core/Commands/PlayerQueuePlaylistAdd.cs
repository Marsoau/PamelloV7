using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class PlayerQueuePlaylistAdd : PamelloCommand
{
    public IEnumerable<IPamelloPlaylist> Execute(IEnumerable<IPamelloPlaylist> playlists, string position = "last") {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.InsertPlaylist(position, playlists, ScopeUser);
    }
}

