using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class SongFavoritesMove : PamelloCommand
{
    public IPamelloSong? Execute(int fromPosition, int toPosition) {
        return ScopeUser.MoveFavoriteSong(fromPosition, toPosition);
    }
}
