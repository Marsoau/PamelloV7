using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class UserFavoriteSongsUpdated : PamelloEvent
    {
        public UserFavoriteSongsUpdated() : base(EEventName.UserFavoriteSongsUpdated) { }

        public int UserId { get; set; }
        public IEnumerable<int> FavoriteSongsIds { get; set; }
    }
}

