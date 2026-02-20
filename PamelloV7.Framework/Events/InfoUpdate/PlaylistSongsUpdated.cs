using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.InfoUpdate;

[Broadcast]
[PamelloEventCategory(EEventCategory.InfoUpdate)]
public class PlaylistSongsUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloPlaylist Playlist { get; set; }
    public IEnumerable<IPamelloSong> Songs { get; set; }
}
