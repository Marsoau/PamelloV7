using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class PlayerQueuePlaylistAdd : PamelloCommand
{
    public IEnumerable<IPamelloPlaylist> Execute(IEnumerable<IPamelloPlaylist> playlists, string position = "last") {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.InsertPlaylist(position, playlists, ScopeUser);
    }
}

