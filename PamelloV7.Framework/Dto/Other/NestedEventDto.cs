using System.Text.Json;
using PamelloV7.Framework.Events.Base;

namespace PamelloV7.Framework.Dto.Other;

public class NestedEventDto
{
    public required string EventName { get; set; }
    public required IPamelloEvent EventData { get; set; }
    public required List<NestedEventDto> NestedEvents { get; set; }
}
