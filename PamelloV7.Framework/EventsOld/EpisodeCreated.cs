using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class EpisodeCreated : PamelloEvent
    {
        public EpisodeCreated() : base(EEventName.EpisodeCreated) { }

        public int EpisodeId { get; set; }
    }
}

