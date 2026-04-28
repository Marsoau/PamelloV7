using PamelloV7.Core.Dto.Entities;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Dto;
using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.Entities;

[PamelloEntity("playlists", typeof(PamelloPlaylistDto))]
public interface IPamelloPlaylist : IPamelloDatabaseEntity
{
    public bool IsProtected { get; }
    public IPamelloUser Owner { get; }
    
    public IReadOnlyList<IPamelloSong> Songs { get; }
    public IReadOnlyList<IPamelloUser> FavoriteBy { get; }
    
    public DateTime AddedAt { get; }
    
    public bool SetIsProtected(bool state, IPamelloUser? scopeUser);
    public IPamelloUser TransferOwnership(IPamelloUser newOwner, IPamelloUser? scopeUser);
    
    public IEnumerable<IPamelloSong> AddSongs(IEnumerable<IPamelloSong> songs, IPamelloUser? scopeUser, int? position = null, bool fromInside = false);
    public IPamelloSong? MoveSong(string fromPosition, string toPosition, IPamelloUser? scopeUser);
    public IEnumerable<IPamelloSong> ReplaceSongs(IEnumerable<IPamelloSong> newSongs, IPamelloUser? scopeUser);
    
    public int RemoveSongs(IEnumerable<IPamelloSong> songs, IPamelloUser? scopeUser, bool fromInside = false);
    public IPamelloSong? RemoveAt(string position, IPamelloUser? scopeUser);
    
    public void MakeFavorite(IPamelloUser user, bool fromInside = false, bool automatic = false);
    public void UnmakeFavorite(IPamelloUser user, int fromInsidePosition = -1, bool automatic = false);
}
