//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.InfoUpdate.EpisodeAutoSkipUpdated")]
public class EpisodeAutoSkipUpdated : RemoteEvent
{
    public System.Boolean AutoSkip { get; set; }
    public System.Int32 Invoker { get; set; }
    public System.Int32? Episode { get; set; }

}