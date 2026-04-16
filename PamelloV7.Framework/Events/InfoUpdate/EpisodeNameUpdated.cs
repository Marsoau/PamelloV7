using PamelloV7.Core.Dto.Entities;
using PamelloV7.Core.Entities.Attributes;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.InfoUpdate;

[Broadcast]
[PamelloEventCategory(EEventCategory.InfoUpdate)]

[Safe<IPamelloEpisode>("Episode")]

[EntityInfoUpdate<IPamelloEpisode>(nameof(Episode),
    nameof(PamelloEpisodeDto.Name)
)]
public partial class EpisodeNameUpdated : IPamelloEvent;

