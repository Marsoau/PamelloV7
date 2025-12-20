using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class SongFavoriteByIdsUpdated : PamelloEvent
    {
        public SongFavoriteByIdsUpdated() : base(EEventName.SongFavoriteByIdsUpdated) { }

        public int SongId { get; set; }
        public IEnumerable<int> FavoriteByIds { get; set; }
    }
}

