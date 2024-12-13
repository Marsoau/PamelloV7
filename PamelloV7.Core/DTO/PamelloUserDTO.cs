using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Core.DTO
{
    public class PamelloUserDTO : IPamelloDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
		public string? AvatarUrl { get; set; }
		public ulong DiscordId { get; set; }
		public int? SelectedPlayerId { get; set; }
        public int SongsPlayed { get; set; }

        public IEnumerable<int> AddedSongsIds { get; set; }
        public IEnumerable<int> AddedPlaylistsIds { get; set; }
        public IEnumerable<int> FavoriteSongsIds { get; set; }
        public IEnumerable<int> FavoritePlaylistsIds { get; set; }

		public bool IsAdministrator { get; set; }
    }
}
