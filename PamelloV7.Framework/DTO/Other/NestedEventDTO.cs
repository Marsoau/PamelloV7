using System.Text.Json;
using PamelloV7.Framework.Events.Base;

namespace PamelloV7.Framework.DTO.Other;

public class NestedEventDTO
{
    public string EventName { get; set; }
    public IPamelloEvent EventData { get; set; }
    public List<NestedEventDTO> NestedEvents { get; set; }
}
