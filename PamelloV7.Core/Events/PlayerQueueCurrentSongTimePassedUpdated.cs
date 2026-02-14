using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Events.Enumerators;

namespace PamelloV7.Core.Events;

[BroadcastToPlayer]
[PamelloEventCategory(EEventCategory.InfoUpdate)]
public class PlayerQueueCurrentSongTimePassedUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloPlayer Player { get; set; }
    public int CurrentSongTimePassed { get; set; }
}
