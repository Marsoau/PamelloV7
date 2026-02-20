using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class SongEpisodesIdsUpdated : PamelloEvent
    {
        public SongEpisodesIdsUpdated() : base(EEventName.SongEpisodesIdsUpdated) { }

        public int SongId { get; set; }
        public IEnumerable<int> EpisodesIds { get; set; }
    }
}

