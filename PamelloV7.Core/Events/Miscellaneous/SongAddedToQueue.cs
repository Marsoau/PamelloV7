using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Events.Enumerators;
using PamelloV7.Core.Events.InfoUpdate;
using PamelloV7.Core.Events.RestorePacks;

namespace PamelloV7.Core.Events.Miscellaneous;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Miscellaneous)]
public class SongAddedToQueue : RevertiblePamelloEvent, IPamelloEvent
{
    public IPamelloPlayer Player { get; set; }
    public IEnumerable<IPamelloSong> Songs { get; set; }
    public int QueuePosition { get; set; }
}

