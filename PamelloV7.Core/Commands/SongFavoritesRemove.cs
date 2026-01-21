using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;

namespace PamelloV7.Core.Commands;

public class SongFavoritesRemove : PamelloCommand
{
    public List<IPamelloSong> Execute(List<IPamelloSong> songs) {
        ScopeUser.StartChangesAsync().Wait();
        songs.ForEach(song => song.UnmakeFavorite(ScopeUser));
        ScopeUser.EndChanges();
        
        return songs;
    }
}
