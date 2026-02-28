//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.EventsConnected")]
public class EventsConnected : IRemoteEvent
{
    public System.Guid EventsToken { get; set; }
    public System.Int32? Invoker { get; set; }

}