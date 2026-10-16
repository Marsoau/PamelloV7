//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.Actions.UserFavoritePlaylistsRemoved")]
public class UserFavoritePlaylistsRemoved : UserFavoritePlaylistsUpdated
{
    public IEnumerable<System.Int32> RemovedPlaylists { get; set; }

}