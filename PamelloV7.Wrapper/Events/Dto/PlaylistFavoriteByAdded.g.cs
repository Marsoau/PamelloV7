//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.Actions.PlaylistFavoriteByAdded")]
public class PlaylistFavoriteByAdded : PlaylistFavoriteByUpdated
{
    public System.Int32? AddedUser { get; set; }

}