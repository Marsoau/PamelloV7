using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Commands;

public class SongFavoritesRemove : PamelloCommand
{
    public List<IPamelloSong> Execute(IEnumerable<IPamelloSong> songs) {
        var removedSongs = new List<IPamelloSong>();
        
        ScopeUser.StartChanges();
        foreach (var song in songs) {
            if (!ScopeUser.FavoriteSongs.Contains(song)) continue;
            
            song.UnmakeFavorite(ScopeUser);
            removedSongs.Add(song);
        }
        ScopeUser.EndChanges();
        
        return removedSongs;
    }
}
