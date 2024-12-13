using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlayerQueueIsReversedUpdated : PamelloEvent
    {
        public PlayerQueueIsReversedUpdated() : base(EEventName.PlayerQueueIsReversedUpdated) { }

        public bool IsReversed { get; set; }
    }
}

