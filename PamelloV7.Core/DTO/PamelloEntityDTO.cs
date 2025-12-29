using System.Text.Json.Serialization;

namespace PamelloV7.Core.DTO;

public record PamelloEntityDTO : IPamelloDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
