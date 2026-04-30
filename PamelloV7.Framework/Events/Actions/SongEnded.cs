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
    public required int TotalTime { get; set; }
    public required int TotalTimePlayed { get; set; }
    
    //userId, timeSeconds
    public required Dictionary<int, int> UserTimeListened { get; set; }
}
