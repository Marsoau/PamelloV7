using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events;

[Broadcast]
public class SongNameUpdated : IPamelloEvent
{
    public IPamelloSong Song { get; set; }
    public string NewName { get; set; }
}
