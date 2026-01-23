using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events;

[Broadcast]
[InfoUpdate]
public class PlaylistFavoriteByUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloPlaylist Playlist { get; set; }
    public IEnumerable<IPamelloUser> FavoriteBy { get; set; }
}
