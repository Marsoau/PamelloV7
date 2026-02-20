using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class UserFavoriteSongsUpdated : PamelloEvent
    {
        public UserFavoriteSongsUpdated() : base(EEventName.UserFavoriteSongsUpdated) { }

        public int UserId { get; set; }
        public IEnumerable<int> FavoriteSongsIds { get; set; }
    }
}

