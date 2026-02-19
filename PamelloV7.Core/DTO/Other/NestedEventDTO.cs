using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.DTO.Other;

public class NestedEventDTO
{
    public string EventName { get; set; }
    public IPamelloEvent EventData { get; set; }
    public List<NestedEventDTO> NestedEvents { get; set; }
}
