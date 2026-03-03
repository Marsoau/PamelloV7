using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloUserDto : PamelloEntityDto
    {
        [JsonPropertyName("avatarUrl")]
		public string? AvatarUrl { get; set; }

        [JsonPropertyName("selectedPlayerId")]
		public int? SelectedPlayerId { get; set; }
		
		[JsonPropertyName("selectedAuthorizationPos")]
		public int? SelectedAuthorizationIndex { get; set; }

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
        
        
        [JsonPropertyName("authorizationsPlatformKeys")]
        public IEnumerable<string> AuthorizationsPlatformKeys { get; set; }

        
        [JsonPropertyName("isAdministrator")]
		public bool IsAdministrator { get; set; }
    }
}
