using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class EpisodeSkipUpdated : PamelloEvent
    {
        public EpisodeSkipUpdated() : base(EEventName.EpisodeSkipUpdated) { }

        public int EpisodeId { get; set; }
        public bool Skip { get; set; }
    }
}

