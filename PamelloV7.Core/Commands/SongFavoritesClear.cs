using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class SongFavoritesClear : PamelloCommand
{
    public IEnumerable<IPamelloSong> Execute() {
        return ScopeUser.ClearFavoriteSongs();
    }
}
