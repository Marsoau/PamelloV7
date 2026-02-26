using PamelloV7.Core.Entities.Attributes;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.Destructive;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Destructive)]

[SafeEntity<IPamelloSong>("Song")]
public partial class SongDeleted : IRevertiblePamelloEvent, IPamelloEvent;
