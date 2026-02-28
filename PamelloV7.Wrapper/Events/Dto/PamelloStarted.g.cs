//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.InfoUpdate.PamelloStarted")]
public class PamelloStarted : IRemoteEvent
{
    public System.IServiceProvider Services { get; set; }
    public System.Int32? Invoker { get; set; }

}