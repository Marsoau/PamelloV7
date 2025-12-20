using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class PlayerQueuePositionUpdated : PamelloEvent
    {
        public PlayerQueuePositionUpdated() : base(EEventName.PlayerQueuePositionUpdated) { }

        public int QueuePosition { get; set; }
    }
}

