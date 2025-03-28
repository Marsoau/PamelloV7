using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PamelloV7.Core.DTO
{
    public class PamelloQueueEntryDTO
    {
        [JsonPropertyName("songId")]
        public int SongId { get; set; }

        [JsonPropertyName("adderId")]
        public int? AdderId { get; set; }
    }
}
