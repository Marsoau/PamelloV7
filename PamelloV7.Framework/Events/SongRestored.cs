using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Creative)]
public class SongRestored : RevertiblePamelloEvent, IPamelloEvent
{
    public IPamelloSong Song { get; set; }
}

