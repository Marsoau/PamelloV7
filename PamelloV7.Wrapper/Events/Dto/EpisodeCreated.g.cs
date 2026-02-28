//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.Creative.EpisodeCreated")]
public class EpisodeCreated : IRemoteEvent
{
    public System.Int32? Invoker { get; set; }
    public System.Int32? Episode { get; set; }

}