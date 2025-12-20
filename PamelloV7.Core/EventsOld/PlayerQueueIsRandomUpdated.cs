using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class PlayerQueueIsRandomUpdated : PamelloEvent
    {
        public PlayerQueueIsRandomUpdated() : base(EEventName.PlayerQueueIsRandomUpdated) { }

        public bool IsRandom { get; set; }
    }
}

