using PamelloV7.Framework.Containers;

namespace PamelloV7.Wrapper.Entities.Dto.Other;

public class RemoteQueueEntryDto
{
    public required Safe<RemoteSong> Song { get; set; }
    public required Safe<RemoteSong> Adder { get; set; }
}
