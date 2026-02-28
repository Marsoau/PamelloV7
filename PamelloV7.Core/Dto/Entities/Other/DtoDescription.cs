using System.Text.Json.Nodes;

namespace PamelloV7.Core.Dto.Entities.Other;

public class DtoDescription
{
    public int EntityId { get; set; }
    public string EntityType { get; set; }
    public JsonNode Data { get; set; }
}
