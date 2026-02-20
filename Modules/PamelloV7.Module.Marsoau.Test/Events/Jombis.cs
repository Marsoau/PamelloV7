using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Core.Events;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Miscellaneous)]
public class Jombis : IPamelloEvent
{
    public List<IPamelloSong> Songs { get; set; }
    public IPamelloSong Song { get; set; }
    public string Message { get; set; }
}
