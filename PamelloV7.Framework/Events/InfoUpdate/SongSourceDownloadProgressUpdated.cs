using PamelloV7.Core.Entities.Attributes;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.InfoUpdate;

[Broadcast]
[PamelloEventCategory(EEventCategory.InfoUpdate)]

[SafeEntity<IPamelloSong>("Song", typeof(InfoUpdatePropertyAttribute))]
public partial class SongSourceDownloadProgressUpdated : IPamelloEvent
{
    public int SourceIndex { get; set; }
    public double Progress { get; set; }
}
