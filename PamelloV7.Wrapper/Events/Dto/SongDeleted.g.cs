//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.Destructive.SongDeleted")]
public class SongDeleted : IRemoteEvent
{
    public System.Int32 Invoker { get; set; }
    public System.Int32 Song { get; set; }

}