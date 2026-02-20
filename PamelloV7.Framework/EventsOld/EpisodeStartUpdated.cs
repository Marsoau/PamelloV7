using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class EpisodeStartUpdated : PamelloEvent
    {
        public EpisodeStartUpdated() : base(EEventName.EpisodeStartUpdated) { }

        public int EpisodeId { get; set; }
        public int Start { get; set; }
    }
}

