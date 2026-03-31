using System.Text.Json.Nodes;

namespace PamelloV7.Core.Dto.Entities.Other;

public class DtoDescription
{
    public required int EntityId { get; set; }
    public required string EntityType { get; set; }
    public required JsonNode Data { get; set; }
}
