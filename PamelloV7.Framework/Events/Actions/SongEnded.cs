using PamelloV7.Core.Audio;
using PamelloV7.Core.Entities.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.Actions;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Action)]

[Safe<IPamelloSong>("Song")]
public partial class SongEnded : IPamelloEvent
{
    public required AudioTime TotalTimeListened { get; set; }
    public required Dictionary<IPamelloUser, AudioTime> UserTimeListened { get; set; }
}
