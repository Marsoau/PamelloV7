using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlayerQueueIsFeedRandomUpdated : PamelloEvent
    {
        public PlayerQueueIsFeedRandomUpdated() : base(EEventName.PlayerQueueIsFeedRandomUpdated) { }

        public bool IsFeedRandom { get; set; }
    }
}

