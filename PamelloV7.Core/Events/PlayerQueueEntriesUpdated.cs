using PamelloV7.Core.Enumerators;
using PamelloV7.Core.DTO;

namespace PamelloV7.Core.Events
{
    public class PlayerQueueEntriesUpdated : PamelloEvent
    {
        public PlayerQueueEntriesUpdated() : base(EEventName.PlayerQueueEntriesUpdated) { }

        public IEnumerable<PamelloQueueEntryDTO> QueueEntriesDTOs { get; set; }
    }
}

