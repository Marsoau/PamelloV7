using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlayerQueueIsNoLeftoversUpdated : PamelloEvent
    {
        public PlayerQueueIsNoLeftoversUpdated() : base(EEventName.PlayerQueueIsNoLeftoversUpdated) { }

        public bool IsNoLeftovers { get; set; }
    }
}

