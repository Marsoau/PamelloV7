using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlaylistFavoriteByIdsUpdated : PamelloEvent
    {
        public PlaylistFavoriteByIdsUpdated() : base(EEventName.PlaylistFavoriteByIdsUpdated) { }

        public int PlaylistId { get; set; }
        public IEnumerable<int> FavoriteByIds { get; set; }
    }
}

