using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class PlaylistFavoriteClear : PamelloCommand
{
    public IEnumerable<IPamelloPlaylist> Execute() {
        return ScopeUser.ClearFavoritePlaylists();
    }
}
