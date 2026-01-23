using PamelloV7.Core.Attributes;
using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Core.Entities;

[ValueEntity("playlists")]
public interface IPamelloPlaylist : IPamelloDatabaseEntity
{
    public bool IsProtected { get; set; }
    public IPamelloUser Owner { get; }
    
    public IReadOnlyList<IPamelloSong> Songs { get; }
    public IReadOnlyList<IPamelloUser> FavoriteBy { get; }
    
    public IEnumerable<IPamelloSong> AddSongs(IEnumerable<IPamelloSong> songs, int? position = null, bool fromInside = false);
    public IPamelloSong? MoveSong(int fromPosition, int toPosition);
    public IEnumerable<IPamelloSong> ReplaceSongs(IEnumerable<IPamelloSong> newSongs);
    
    public int RemoveSong(IPamelloSong song, bool fromInside = false);
    public IPamelloSong? RemoveAt(int position);
    
    public void MakeFavorite(IPamelloUser user, bool fromInside = false);
    public void UnmakeFavorite(IPamelloUser user, bool fromInside = false);
}
