using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloPlaylistDto : PamelloEntityDto
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
