using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloEpisodeDto : PamelloEntityDto
    {
        [JsonPropertyName("start")]
        public int Start { get; set; }

        [JsonPropertyName("skip")]
        public bool AutoSkip { get; set; }

        [JsonPropertyName("songId")]
        public int SongId { get; set; }
    }
}
