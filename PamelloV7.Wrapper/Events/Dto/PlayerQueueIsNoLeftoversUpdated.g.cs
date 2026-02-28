//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.InfoUpdate.PlayerQueueIsNoLeftoversUpdated")]
public class PlayerQueueIsNoLeftoversUpdated : IRemoteEvent
{
    public System.Boolean IsNoLeftovers { get; set; }
    public System.Int32? Invoker { get; set; }
    public System.Int32? Player { get; set; }

}