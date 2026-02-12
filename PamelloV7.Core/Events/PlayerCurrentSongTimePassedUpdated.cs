using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events;

[Broadcast]
[InfoUpdate]
public class PlayerCurrentSongTimePassedUpdated : IPamelloEvent
{
    public int CurrentSongTimePassed { get; set; }
}
