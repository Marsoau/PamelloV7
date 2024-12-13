using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class UserFavoritePlaylistsUpdated : PamelloEvent
    {
        public UserFavoritePlaylistsUpdated() : base(EEventName.UserFavoritePlaylistsUpdated) { }

        public int UserId { get; set; }
        public IEnumerable<int> FavoritePlaylistsIds { get; set; }
    }
}

