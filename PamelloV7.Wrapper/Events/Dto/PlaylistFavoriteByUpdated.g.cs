//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.InfoUpdate.PlaylistFavoriteByUpdated")]
public class PlaylistFavoriteByUpdated : IRemoteEvent
{
    public IEnumerable<System.Int32> FavoriteBy { get; set; }
    public System.Int32? Invoker { get; set; }
    public System.Int32? Playlist { get; set; }

}