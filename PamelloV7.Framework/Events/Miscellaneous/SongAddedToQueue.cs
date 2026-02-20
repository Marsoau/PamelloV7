using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Framework.Events.RestorePacks;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.Miscellaneous;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Miscellaneous)]
public class SongAddedToQueue : RevertiblePamelloEvent, IPamelloEvent
{
    public IPamelloPlayer Player { get; set; }
    public IEnumerable<IPamelloSong> Songs { get; set; }
    public int QueuePosition { get; set; }
}

