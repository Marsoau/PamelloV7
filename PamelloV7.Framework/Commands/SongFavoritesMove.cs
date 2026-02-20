using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class SongFavoritesMove : PamelloCommand
{
    public IPamelloSong? Execute(int fromPosition, int toPosition) {
        return ScopeUser.MoveFavoriteSong(fromPosition, toPosition);
    }
}
