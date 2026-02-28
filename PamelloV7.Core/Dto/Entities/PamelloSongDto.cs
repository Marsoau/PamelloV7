using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloSongDto : PamelloEntityDto
    {
        [JsonPropertyName("coverUrl")]
        public string CoverUrl { get; set; }

        [JsonPropertyName("addedById")]
        public int AddedById { get; set; }

        [JsonPropertyName("addedAt")]
        public DateTime AddedAt { get; set; }


        [JsonPropertyName("associations")]
        public IEnumerable<string> Associations { get; set; }

        [JsonPropertyName("favoriteByIds")]
        public IEnumerable<int> FavoriteByIds { get; set; }

        [JsonPropertyName("episodesIds")]
        public IEnumerable<int> EpisodesIds { get; set; }

        [JsonPropertyName("playlistsIds")]
        public IEnumerable<int> PlaylistsIds { get; set; }

        
        [JsonPropertyName("sourcesPlatformKeys")]
        public IEnumerable<string> SourcesPlatformKeys { get; set; }
    }
}
