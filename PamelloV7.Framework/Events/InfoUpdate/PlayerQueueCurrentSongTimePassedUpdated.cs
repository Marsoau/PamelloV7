using PamelloV7.Core.Dto.Entities;
using PamelloV7.Core.Entities.Attributes;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.DTO;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.InfoUpdate;

[BroadcastToPlayer]
[PamelloEventCategory(EEventCategory.InfoUpdate)]

[SafeEntity<IPamelloPlayer>("Player")]

[EntityInfoUpdate<IPamelloPlayer>(nameof(Player),
    nameof(PamelloPlayerDto.Queue),
    nameof(PamelloPlayerDto.Queue.CurrentSongTimePassed)
)]
public partial class PlayerQueueCurrentSongTimePassedUpdated : IPamelloEvent;
