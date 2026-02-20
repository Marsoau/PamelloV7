using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class EpisodeNameUpdated : PamelloEvent
    {
        public EpisodeNameUpdated() : base(EEventName.EpisodeNameUpdated) { }

        public int EpisodeId { get; set; }
        public string Name { get; set; }
    }
}

