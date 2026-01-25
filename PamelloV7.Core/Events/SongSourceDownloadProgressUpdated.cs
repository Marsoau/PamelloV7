using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events;

[Broadcast]
[InfoUpdate]
public class SongSourceDownloadProgressUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloSong Song { get; set; }
    public int SourceIndex { get; set; }
    public double Progress { get; set; }
}
