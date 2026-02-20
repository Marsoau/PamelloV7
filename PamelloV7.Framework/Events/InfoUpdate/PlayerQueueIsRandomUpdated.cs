using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.InfoUpdate;

[BroadcastToPlayer]
[PamelloEventCategory(EEventCategory.InfoUpdate)]
public partial class PlayerQueueIsRandomUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloPlayer Player { get; set; }
    public bool IsRandom { get; set; }
}

