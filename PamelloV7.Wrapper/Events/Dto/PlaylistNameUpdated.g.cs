//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.InfoUpdate.PlaylistNameUpdated")]
public class PlaylistNameUpdated : IRemoteEvent
{
    public System.String NewName { get; set; }
    public System.Int32 Invoker { get; set; }
    public System.Int32 Playlist { get; set; }

}