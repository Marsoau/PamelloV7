using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class SongEpisodesIdsUpdated : PamelloEvent
    {
        public SongEpisodesIdsUpdated() : base(EEventName.SongEpisodesIdsUpdated) { }

        public int SongId { get; set; }
        public IEnumerable<int> EpisodesIds { get; set; }
    }
}

