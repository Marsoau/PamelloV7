using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloUserDto : PamelloEntityDto
    {
		public string? AvatarUrl { get; set; }
		public int? SelectedPlayerId { get; set; }
		public int? SelectedAuthorizationIndex { get; set; }

        public DateTime JoinedAt { get; set; }

        public IEnumerable<int> AddedSongsIds { get; set; }

        public IEnumerable<int> AddedPlaylistsIds { get; set; }
        public IEnumerable<int> FavoriteSongsIds { get; set; }
        public IEnumerable<int> FavoritePlaylistsIds { get; set; }
        
        public IEnumerable<string> AuthorizationsPlatformKeys { get; set; }
        
		public bool IsAdministrator { get; set; }
    }
}
