using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.Entities;

[ValueEntity("playlists")]
public interface IPamelloPlaylist : IPamelloDatabaseEntity
{
    public bool IsProtected { get; }
    public IPamelloUser Owner { get; }
    
    public IReadOnlyList<IPamelloSong> Songs { get; }
    public IReadOnlyList<IPamelloUser> FavoriteBy { get; }
    
    public IEnumerable<IPamelloSong> AddSongs(IEnumerable<IPamelloSong> songs, IPamelloUser? scopeUser, int? position = null, bool fromInside = false);
    public IPamelloSong? MoveSong(int fromPosition, int toPosition, IPamelloUser? scopeUser);
    public IEnumerable<IPamelloSong> ReplaceSongs(IEnumerable<IPamelloSong> newSongs, IPamelloUser? scopeUser);
    
    public int RemoveSong(IPamelloSong song, IPamelloUser? scopeUser, bool fromInside = false);
    public IPamelloSong? RemoveAt(int position, IPamelloUser? scopeUser);
    
    public void MakeFavorite(IPamelloUser user, bool fromInside = false, bool automatic = false);
    public void UnmakeFavorite(IPamelloUser user, bool fromInside = false, bool automatic = false);
}
