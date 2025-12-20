using PamelloV7.Core.DTO;
using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class PlayerQueueEntriesDTOsUpdated : PamelloEvent
    {
        public PlayerQueueEntriesDTOsUpdated() : base(EEventName.PlayerQueueEntriesDTOsUpdated) { }

        public IEnumerable<PamelloQueueEntryDTO> QueueEntriesDTOs { get; set; }
    }
}

