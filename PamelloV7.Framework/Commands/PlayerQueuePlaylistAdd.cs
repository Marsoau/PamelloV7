using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class PlayerQueuePlaylistAdd
{
    public IEnumerable<IPamelloPlaylist> Execute(IEnumerable<IPamelloPlaylist> playlists, string position = "last") {
        return ScopeUser.RequiredSelectedPlayer.RequiredQueue.InsertPlaylist(position, playlists, ScopeUser);
    }
}

