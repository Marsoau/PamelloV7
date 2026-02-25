//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.InfoUpdate.SongSourceDownloadProgressUpdated")]
public class SongSourceDownloadProgressUpdated : IRemoteEvent
{
    public System.Int32 SourceIndex { get; set; }
    public System.Double Progress { get; set; }
    public System.Int32 Invoker { get; set; }
    public System.Int32 Song { get; set; }

}