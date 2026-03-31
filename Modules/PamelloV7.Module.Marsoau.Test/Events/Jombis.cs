using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Core.Events;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Miscellaneous)]
public partial class Jombis : IPamelloEvent
{
    public required List<IPamelloSong> Songs { get; set; }
    public required IPamelloSong Song { get; set; }
    public required string Message { get; set; }
}
