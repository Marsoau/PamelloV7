using System.Text.Json.Serialization;
using PamelloV7.Core.Dto.Entities.Other;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloPlayerDto : PamelloEntityDto
    {
        [JsonPropertyName("ownerId")]
        public int OwnerId { get; set; }

        [JsonPropertyName("isProtected")]
        public bool IsProtected { get; set; }

        [JsonPropertyName("isPaused")]
        public bool IsPaused { get; set; }
        
        [JsonPropertyName("queue")]
        public PamelloQueueDto Queue { get; set; }
    }
}
