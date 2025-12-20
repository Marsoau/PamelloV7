using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class EpisodeDeleted : PamelloEvent
    {
        public EpisodeDeleted() : base(EEventName.EpisodeDeleted) { }

        public int EpisodeId { get; set; }
    }
}

