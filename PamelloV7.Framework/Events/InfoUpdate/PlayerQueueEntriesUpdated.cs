using PamelloV7.Core.Dto.Entities;
using PamelloV7.Core.Entities.Attributes;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Dto;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.InfoUpdate;

[BroadcastToPlayer]
[PamelloEventCategory(EEventCategory.InfoUpdate)]

[Safe<IPamelloPlayer>("Player", true)]

[EntityInfoUpdate<IPamelloPlayer>(nameof(Player),
    nameof(PamelloPlayerDto.Queue),
    nameof(PamelloPlayerDto.Queue.Entries)
)]
public partial class PlayerQueueEntriesUpdated : IPamelloEvent;

