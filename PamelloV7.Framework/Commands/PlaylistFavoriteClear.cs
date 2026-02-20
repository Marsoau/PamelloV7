using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class PlaylistFavoriteClear : PamelloCommand
{
    public IEnumerable<IPamelloPlaylist> Execute() {
        return ScopeUser.ClearFavoritePlaylists();
    }
}
