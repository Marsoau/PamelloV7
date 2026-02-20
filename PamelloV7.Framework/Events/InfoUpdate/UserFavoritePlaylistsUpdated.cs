using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.InfoUpdate;

[Broadcast]
[PamelloEventCategory(EEventCategory.InfoUpdate)]
public partial class UserFavoritePlaylistsUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloUser User { get; set; }
    public IEnumerable<IPamelloPlaylist> FavoritePlaylists { get; set; }
}
