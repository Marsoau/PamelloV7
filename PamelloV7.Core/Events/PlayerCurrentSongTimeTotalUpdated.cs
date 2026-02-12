using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events;

[Broadcast]
[InfoUpdate]
public class PlayerCurrentSongTimeTotalUpdated : IPamelloEvent
{
    public int CurrentSongTimeTotal { get; set; }
}
