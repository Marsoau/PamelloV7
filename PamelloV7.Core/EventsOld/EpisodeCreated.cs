using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class EpisodeCreated : PamelloEvent
    {
        public EpisodeCreated() : base(EEventName.EpisodeCreated) { }

        public int EpisodeId { get; set; }
    }
}

