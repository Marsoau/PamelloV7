using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto
{
    public record IPamelloDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
