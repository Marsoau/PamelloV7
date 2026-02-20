using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class EpisodeDeleted : PamelloEvent
    {
        public EpisodeDeleted() : base(EEventName.EpisodeDeleted) { }

        public int EpisodeId { get; set; }
    }
}

