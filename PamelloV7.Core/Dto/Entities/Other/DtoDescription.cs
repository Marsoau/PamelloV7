using System.Text.Json.Nodes;

namespace PamelloV7.Core.Dto.Entities.Other;

public class DtoDescription
{
    public string Type { get; set; }
    public JsonNode Data { get; set; }
}
