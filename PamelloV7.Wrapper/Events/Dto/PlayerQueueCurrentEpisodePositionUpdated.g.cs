//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.InfoUpdate.PlayerQueueCurrentEpisodePositionUpdated")]
public class PlayerQueueCurrentEpisodePositionUpdated : RemoteEvent
{
    public System.Int32 CurrentEpisodePosition { get; set; }
    public System.Int32 Invoker { get; set; }
    public System.Int32? Player { get; set; }

}