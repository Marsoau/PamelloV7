using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class EpisodeUpdated : PamelloEvent
    {
        public EpisodeUpdated() : base(EEventName.EpisodeUpdated) { }
    }
}

