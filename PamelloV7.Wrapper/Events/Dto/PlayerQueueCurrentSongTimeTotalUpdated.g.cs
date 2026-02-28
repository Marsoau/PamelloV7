//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.InfoUpdate.PlayerQueueCurrentSongTimeTotalUpdated")]
public class PlayerQueueCurrentSongTimeTotalUpdated : IRemoteEvent
{
    public System.Int32 CurrentSongTimeTotal { get; set; }
    public System.Int32? Invoker { get; set; }
    public System.Int32? Player { get; set; }

}