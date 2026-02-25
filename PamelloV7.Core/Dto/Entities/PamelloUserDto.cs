using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public record PamelloUserDto : PamelloEntityDto
    {
        [JsonPropertyName("avatarUrl")]
		public string? AvatarUrl { get; set; }

        [JsonPropertyName("selectedPlayerId")]
		public int? SelectedPlayerId { get; set; }
		
		[JsonPropertyName("selectedAuthorizationPos")]
		public int? SelectedAuthorizationPos { get; set; }

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
        
        
        [JsonPropertyName("authorizationsPlatfromKeys")]
        public IEnumerable<string> AuthorizationsPlatfromKeys { get; set; }

        
        [JsonPropertyName("isAdministrator")]
		public bool IsAdministrator { get; set; }
    }
}
