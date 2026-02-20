using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlaylistFavoriteByIdsUpdated : PamelloEvent
    {
        public PlaylistFavoriteByIdsUpdated() : base(EEventName.PlaylistFavoriteByIdsUpdated) { }

        public int PlaylistId { get; set; }
        public IEnumerable<int> FavoriteByIds { get; set; }
    }
}

