using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlayerQueueSongsIdsUpdated : PamelloEvent
    {
        public PlayerQueueSongsIdsUpdated() : base(EEventName.PlayerQueueSongsIdsUpdated) { }

        public IEnumerable<int> QueueSongsIds { get; set; }
    }
}

