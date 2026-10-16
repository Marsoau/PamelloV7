//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.Actions.SongEnded")]
public class SongEnded : RemoteEvent
{
    public System.Int32 TotalTime { get; set; }
    public System.Int32 TotalTimePlayed { get; set; }
    public IEnumerable<System.Int32> UserTimeListened { get; set; }
    public System.Int32 Invoker { get; set; }
    public System.Int32? Song { get; set; }

}