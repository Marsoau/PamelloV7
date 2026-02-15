using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Events.Enumerators;
using PamelloV7.Core.Events.RestorePacks;
using PamelloV7.Core.Events.RestorePacks.Base;

namespace PamelloV7.Core.Events;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Destructive)]
public class SongDeleted : RevertiblePamelloEvent, IPamelloEvent
{
    public int SongId { get; set; }
}

