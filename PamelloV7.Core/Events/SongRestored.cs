using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Events.Enumerators;

namespace PamelloV7.Core.Events;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Creative)]
public class SongRestored : RestorablePamelloEvent, IPamelloEvent
{
    public IPamelloSong Song { get; set; }
}

