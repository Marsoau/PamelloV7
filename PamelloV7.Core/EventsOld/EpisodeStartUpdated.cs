using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class EpisodeStartUpdated : PamelloEvent
    {
        public EpisodeStartUpdated() : base(EEventName.EpisodeStartUpdated) { }

        public int EpisodeId { get; set; }
        public int Start { get; set; }
    }
}

