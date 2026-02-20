using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlayerQueueIsFeedRandomUpdated : PamelloEvent
    {
        public PlayerQueueIsFeedRandomUpdated() : base(EEventName.PlayerQueueIsFeedRandomUpdated) { }

        public bool QueueIsFeedRandom { get; set; }
    }
}

