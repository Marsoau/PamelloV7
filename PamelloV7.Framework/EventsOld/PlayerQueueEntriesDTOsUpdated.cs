using PamelloV7.Framework.DTO;
using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlayerQueueEntriesDTOsUpdated : PamelloEvent
    {
        public PlayerQueueEntriesDTOsUpdated() : base(EEventName.PlayerQueueEntriesDTOsUpdated) { }

        public IEnumerable<PamelloQueueEntryDTO> QueueEntriesDTOs { get; set; }
    }
}

