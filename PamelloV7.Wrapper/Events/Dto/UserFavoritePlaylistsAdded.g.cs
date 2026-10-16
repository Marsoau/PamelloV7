//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.Actions.UserFavoritePlaylistsAdded")]
public class UserFavoritePlaylistsAdded : UserFavoritePlaylistsUpdated
{
    public IEnumerable<System.Int32> AddedPlaylists { get; set; }

}