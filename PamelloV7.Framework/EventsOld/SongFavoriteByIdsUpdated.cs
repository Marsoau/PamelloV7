using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class SongFavoriteByIdsUpdated : PamelloEvent
    {
        public SongFavoriteByIdsUpdated() : base(EEventName.SongFavoriteByIdsUpdated) { }

        public int SongId { get; set; }
        public IEnumerable<int> FavoriteByIds { get; set; }
    }
}

