using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlayerQueueIsRandomUpdated : PamelloEvent
    {
        public PlayerQueueIsRandomUpdated() : base(EEventName.PlayerQueueIsRandomUpdated) { }

        public bool IsRandom { get; set; }
    }
}

