using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events;

[Broadcast]
public class SongDeleted : IPamelloEvent
{
    public int SongId { get; set; }
}

