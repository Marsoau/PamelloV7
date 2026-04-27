using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class SongFavoritesAdd
{
    public List<IPamelloSong> Execute(IEnumerable<IPamelloSong> songs) {
        return ScopeUser.AddFavoriteSongs(songs).ToList();
    }
}
