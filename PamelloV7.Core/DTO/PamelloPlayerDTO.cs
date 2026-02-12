using PamelloV7.Core.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PamelloV7.Core.DTO.Other;
using PamelloV7.Core.Entities.Other;

namespace PamelloV7.Core.DTO
{
    public record PamelloPlayerDTO : IPamelloDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("ownerId")]
        public int OwnerId { get; set; }

        [JsonPropertyName("isProtected")]
        public bool IsProtected { get; set; }

        [JsonPropertyName("state")]
        public EPlayerState State { get; set; }

        [JsonPropertyName("isPaused")]
        public bool IsPaused { get; set; }
        
        [JsonPropertyName("queue")]
        public PamelloQueueDTO Queue { get; set; }
    }
}
