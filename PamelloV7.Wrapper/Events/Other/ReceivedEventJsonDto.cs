using System.Text.Json;
using PamelloV7.Core.Dto.Signal;

namespace PamelloV7.Wrapper.Events.Other;

public class ReceivedEventJsonDto
{
    public EventTypeInfo Type { get; set; }
    public List<EventTypeInfo> NestedTypes { get; set; }
    
    public JsonElement Data { get; set; } 
}
