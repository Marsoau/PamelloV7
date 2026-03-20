using PamelloV7.Framework.Containers;

namespace PamelloV7.Wrapper.Entities.Dto.Other;

public class RemoteQueueEntryDto
{
    public required SafeStoredEntity<RemoteSong> Song { get; set; }
    public required SafeStoredEntity<RemoteSong> Adder { get; set; }
}
