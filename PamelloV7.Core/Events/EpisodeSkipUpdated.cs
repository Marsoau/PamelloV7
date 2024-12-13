using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class EpisodeSkipUpdated : PamelloEvent
    {
        public EpisodeSkipUpdated() : base(EEventName.EpisodeSkipUpdated) { }

        public int EpisodeId { get; set; }
        public bool Skip { get; set; }
    }
}

