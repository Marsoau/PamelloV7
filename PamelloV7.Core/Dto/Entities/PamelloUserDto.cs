using System.Text.Json.Serialization;

namespace PamelloV7.Core.Dto.Entities
{
    public class PamelloUserDto : PamelloEntityDto
    {
		public required string AvatarUrl { get; set; }
		public required int SelectedPlayer { get; set; }
		public required int SelectedAuthorizationIndex { get; set; }

        public required DateTime JoinedAt { get; set; }

        public required IEnumerable<int> AddedSongs { get; set; }

        public required IEnumerable<int> AddedPlaylists { get; set; }
        public required IEnumerable<int> FavoriteSongs { get; set; }
        public required IEnumerable<int> FavoritePlaylists { get; set; }
        
        public required IEnumerable<string> AuthorizationsPlatformKeys { get; set; }
        
		public required bool IsAdministrator { get; set; }
    }
}
