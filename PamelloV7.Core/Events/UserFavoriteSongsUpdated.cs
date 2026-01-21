using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events;

[Broadcast]
[InfoUpdate]
public class UserFavoriteSongsUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloUser User { get; set; }
    public IEnumerable<IPamelloSong> FavoriteSongs { get; set; }
}
