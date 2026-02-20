using PamelloV7.Framework.DTO;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.InfoUpdate;

[BroadcastToPlayer]
[PamelloEventCategory(EEventCategory.InfoUpdate)]
public partial class PlayerQueueEntriesUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloPlayer Player { get; set; }
    public IEnumerable<PamelloQueueEntry> Entries { get; set; }
}

