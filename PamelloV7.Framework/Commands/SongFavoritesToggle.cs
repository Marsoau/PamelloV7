using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class SongFavoritesToggle
{
    public void Execute(IEnumerable<IPamelloSong> songs) {
        var songsList = songs.ToList();

        var songsToAdd = songsList.Where(song => !song.FavoriteBy.Contains(ScopeUser)).ToList();
        var songsToRemove = songsList.Where(song => song.FavoriteBy.Contains(ScopeUser)).ToList();
        
        ScopeUser.AddFavoriteSongs(songsToAdd);
        ScopeUser.RemoveFavoriteSongs(songsToRemove);
    }
}
