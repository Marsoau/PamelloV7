using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events;

[Broadcast]
public class Jombis : IPamelloEvent
{
    public List<IPamelloSong> Songs { get; set; }
    public IPamelloSong Song { get; set; }
    public string Message { get; set; }
}
