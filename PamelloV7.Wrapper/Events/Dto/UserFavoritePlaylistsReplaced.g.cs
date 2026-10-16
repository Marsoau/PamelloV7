//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.Actions.UserFavoritePlaylistsReplaced")]
public class UserFavoritePlaylistsReplaced : UserFavoritePlaylistsUpdated
{
    public IEnumerable<System.Int32> PreviousFavoritePlaylists { get; set; }
    public IEnumerable<System.Int32> AddedPlaylists { get; set; }
    public IEnumerable<System.Int32> RemovedPlaylists { get; set; }

}