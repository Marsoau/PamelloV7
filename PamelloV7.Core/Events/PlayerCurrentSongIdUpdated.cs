using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events;

[Broadcast]
[InfoUpdate]
public class PlayerCurrentSongIdUpdated : IPamelloEvent
{
    public int? CurrentSongId { get; set; }
}
