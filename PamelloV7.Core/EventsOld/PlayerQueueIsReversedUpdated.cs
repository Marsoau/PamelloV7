using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class PlayerQueueIsReversedUpdated : PamelloEvent
    {
        public PlayerQueueIsReversedUpdated() : base(EEventName.PlayerQueueIsReversedUpdated) { }

        public bool IsReversed { get; set; }
    }
}

