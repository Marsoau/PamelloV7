using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlayerQueueIsReversedUpdated : PamelloEvent
    {
        public PlayerQueueIsReversedUpdated() : base(EEventName.PlayerQueueIsReversedUpdated) { }

        public bool IsReversed { get; set; }
    }
}

