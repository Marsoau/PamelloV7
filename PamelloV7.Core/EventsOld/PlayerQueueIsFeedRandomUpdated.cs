using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class PlayerQueueIsFeedRandomUpdated : PamelloEvent
    {
        public PlayerQueueIsFeedRandomUpdated() : base(EEventName.PlayerQueueIsFeedRandomUpdated) { }

        public bool QueueIsFeedRandom { get; set; }
    }
}

