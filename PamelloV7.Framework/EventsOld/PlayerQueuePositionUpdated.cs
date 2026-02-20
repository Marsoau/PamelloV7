using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlayerQueuePositionUpdated : PamelloEvent
    {
        public PlayerQueuePositionUpdated() : base(EEventName.PlayerQueuePositionUpdated) { }

        public int QueuePosition { get; set; }
    }
}

