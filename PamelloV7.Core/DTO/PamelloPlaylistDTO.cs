using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PamelloV7.Core.DTO
{
    public class PamelloPlaylistDTO : IPamelloDTO
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("addedById")]
        public int AddedById { get; set; }

        [JsonPropertyName("isProtected")]
        public bool IsProtected { get; set; }


        [JsonPropertyName("songsIds")]
        public IEnumerable<int> SongsIds { get; set; }

        [JsonPropertyName("favoriteByIds")]
        public IEnumerable<int> FavoriteByIds { get; set; }
    }
}
