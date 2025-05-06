using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PamelloV7.Core.DTO
{
    public class PamelloUserDTO : IPamelloDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("avatarUrl")]
		public string? AvatarUrl { get; set; }

        [JsonPropertyName("discordId")]
		public ulong DiscordId { get; set; }

        [JsonPropertyName("selectedPlayerId")]
		public int? SelectedPlayerId { get; set; }

        [JsonPropertyName("songsPlayed")]
        public int SongsPlayed { get; set; }

        [JsonPropertyName("joinedAt")]
        public DateTime JoinedAt { get; set; }


        [JsonPropertyName("addedSongsIds")]
        public IEnumerable<int> AddedSongsIds { get; set; }

        [JsonPropertyName("addedPlaylistsIds")]
        public IEnumerable<int> AddedPlaylistsIds { get; set; }

        [JsonPropertyName("favoriteSongsIds")]
        public IEnumerable<int> FavoriteSongsIds { get; set; }

        [JsonPropertyName("favoritePlaylistsIds")]
        public IEnumerable<int> FavoritePlaylistsIds { get; set; }


        [JsonPropertyName("isAdministrator")]
		public bool IsAdministrator { get; set; }
    }
}
