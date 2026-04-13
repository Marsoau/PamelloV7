using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class SongFavoritesAdd
{
    public List<IPamelloSong> Execute(IEnumerable<IPamelloSong> songs) {
        var addedSongs = new List<IPamelloSong>();
        
        ScopeUser.StartChanges();
        foreach (var song in songs) {
            if (ScopeUser.FavoriteSongs.Contains(song)) continue;
            
            song.MakeFavorite(ScopeUser);
            addedSongs.Add(song);
        }
        ScopeUser.EndChanges();
        
        return addedSongs;
    }
}
