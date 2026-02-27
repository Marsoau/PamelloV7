using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PamelloV7.Core.Dto;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.DTO.Other;
using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.DTO
{
    public class PamelloPlayerDTO : PamelloEntityDto
    {
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
