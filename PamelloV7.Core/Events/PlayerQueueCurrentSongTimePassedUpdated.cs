using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events;

[BroadcastToPlayer]
[InfoUpdate]
public class PlayerQueueCurrentSongTimePassedUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloPlayer Player { get; set; }
    public int CurrentSongTimePassed { get; set; }
}
