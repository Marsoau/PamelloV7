using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class SongFavoritesRemove
{
    public List<IPamelloSong> Execute(IEnumerable<IPamelloSong> songs) {
        return ScopeUser.RemoveFavoriteSongs(songs).ToList();
    }
}
