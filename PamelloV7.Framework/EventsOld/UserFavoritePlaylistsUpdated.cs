using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class UserFavoritePlaylistsUpdated : PamelloEvent
    {
        public UserFavoritePlaylistsUpdated() : base(EEventName.UserFavoritePlaylistsUpdated) { }

        public int UserId { get; set; }
        public IEnumerable<int> FavoritePlaylistsIds { get; set; }
    }
}

