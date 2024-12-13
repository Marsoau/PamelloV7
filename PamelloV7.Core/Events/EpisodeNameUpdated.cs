using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class EpisodeNameUpdated : PamelloEvent
    {
        public EpisodeNameUpdated() : base(EEventName.EpisodeNameUpdated) { }

        public int EpisodeId { get; set; }
        public string Name { get; set; }
    }
}

