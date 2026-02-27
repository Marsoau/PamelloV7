using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PamelloV7.Core.Dto;

namespace PamelloV7.Framework.DTO
{
    public class PamelloEpisodeDTO : PamelloEntityDto
    {
        [JsonPropertyName("start")]
        public int Start { get; set; }

        [JsonPropertyName("skip")]
        public bool AutoSkip { get; set; }

        [JsonPropertyName("songId")]
        public int SongId { get; set; }
    }
}
