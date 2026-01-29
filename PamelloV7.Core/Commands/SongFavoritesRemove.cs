using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

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
