using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class SongFavoritesMove
{
    public IPamelloSong? Execute(int fromPosition, int toPosition) {
        return ScopeUser.MoveFavoriteSong(fromPosition, toPosition);
    }
}
