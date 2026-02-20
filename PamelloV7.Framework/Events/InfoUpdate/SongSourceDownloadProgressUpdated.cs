using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.InfoUpdate;

[Broadcast]
[PamelloEventCategory(EEventCategory.InfoUpdate)]
public partial class SongSourceDownloadProgressUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloSong Song { get; set; }
    public int SourceIndex { get; set; }
    public double Progress { get; set; }
}
