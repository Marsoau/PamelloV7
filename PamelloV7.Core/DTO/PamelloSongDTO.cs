using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PamelloV7.Core.DTO
{
    public record PamelloSongDTO : IPamelloDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("youtubeId")]
        public string YoutubeId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("coverUrl")]
        public string CoverUrl { get; set; }

        [JsonPropertyName("playCount")]
        public int PlayCount { get; set; }

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


        [JsonPropertyName("isDownloading")]
        public bool IsDownloading { get; set; }

        [JsonPropertyName("downloadProgress")]
        public double DownloadProgress { get; set; }
    }
}
