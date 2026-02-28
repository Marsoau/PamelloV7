//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.InfoUpdate.PlayerQueueEntriesUpdated")]
public class PlayerQueueEntriesUpdated : IRemoteEvent
{
    public IEnumerable<PamelloV7.Core.Dto.Entities.Other.PamelloQueueEntryDto> Entries { get; set; }
    public System.Int32? Invoker { get; set; }
    public System.Int32? Player { get; set; }

}