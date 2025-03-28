using PamelloV7.Core.Enumerators;
using PamelloV7.Core.DTO;

namespace PamelloV7.Core.Events
{
    public class PlayerQueueEntriesDTOsUpdated : PamelloEvent
    {
        public PlayerQueueEntriesDTOsUpdated() : base(EEventName.PlayerQueueEntriesDTOsUpdated) { }

        public IEnumerable<PamelloQueueEntryDTO> QueueEntriesDTOs { get; set; }
    }
}

