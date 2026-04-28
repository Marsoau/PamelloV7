using PamelloV7.Core.Entities.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Enumerators;
using PamelloV7.Framework.Events.InfoUpdate;

namespace PamelloV7.Framework.Events.Actions;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Action)]

[Safe<IPamelloUser>("RemovedUser", true)]
public partial class PlaylistFavoriteByRemoved : PlaylistFavoriteByUpdated
{
    public required int RemovedFromPosition { get; set; }
}
