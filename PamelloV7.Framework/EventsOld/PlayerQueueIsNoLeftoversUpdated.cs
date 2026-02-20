using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlayerQueueIsNoLeftoversUpdated : PamelloEvent
    {
        public PlayerQueueIsNoLeftoversUpdated() : base(EEventName.PlayerQueueIsNoLeftoversUpdated) { }

        public bool IsNoLeftovers { get; set; }
    }
}

