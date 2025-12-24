using PamelloV7.Core.Attributes;
using PamelloV7.Core.Model.Entities.Base;

namespace PamelloV7.Core.Model.Entities;

[ValueEntity("playlists")]
public interface IPamelloPlaylist : IPamelloDatabaseEntity
{
    public bool IsProtected { get; set; }
    public IPamelloUser Owner { get; }
    
    public IReadOnlyList<IPamelloSong> Songs { get; }
    public IReadOnlyList<IPamelloUser> FavoriteBy { get; }
    
    public IPamelloSong? AddSong(IPamelloSong song, int? position = null, bool fromInside = false);
    public void AddList(IReadOnlyList<IPamelloSong> list, int? position = null);

    public IPamelloSong? MoveSong(int fromPosition, int toPosition);
    public int RemoveSong(IPamelloSong song, bool fromInside = false);
    public IPamelloSong? RemoveAt(int position);
    
    public void MakeFavorite(IPamelloUser user);
    public void UnmakeFavorite(IPamelloUser user);
}
