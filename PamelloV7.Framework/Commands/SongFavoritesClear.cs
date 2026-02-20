using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class SongFavoritesClear : PamelloCommand
{
    public IEnumerable<IPamelloSong> Execute() {
        return ScopeUser.ClearFavoriteSongs();
    }
}
