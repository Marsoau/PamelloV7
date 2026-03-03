using System.Text.Json;
using PamelloV7.Core.Dto.Signal;

namespace PamelloV7.Wrapper.Events.Other;

public class ReceivedEventJsonDto
{
    public List<EventTypeInfo> Types { get; set; }
    
    public JsonElement Data { get; set; } 
}
