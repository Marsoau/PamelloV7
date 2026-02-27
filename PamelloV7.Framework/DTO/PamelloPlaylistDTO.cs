using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PamelloV7.Core.Dto;

namespace PamelloV7.Framework.DTO
{
    public class PamelloPlaylistDTO : PamelloEntityDto
    {
        [JsonPropertyName("ownerId")]
        public int OwnerId { get; set; }

        [JsonPropertyName("isProtected")]
        public bool IsProtected { get; set; }


        [JsonPropertyName("songsIds")]
        public IEnumerable<int> SongsIds { get; set; }

        [JsonPropertyName("favoriteByIds")]
        public IEnumerable<int> FavoriteByIds { get; set; }
    }
}
