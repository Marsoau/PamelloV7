//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.InfoUpdate.SongEpisodesUpdated")]
public class SongEpisodesUpdated : RemoteEvent
{
    public IEnumerable<System.Int32> Episodes { get; set; }
    public System.Int32 Invoker { get; set; }
    public System.Int32? Song { get; set; }

}