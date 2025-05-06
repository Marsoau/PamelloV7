using PamelloV7.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Model
{
    public class RemoteUser : RemoteEntity<PamelloUserDTO>
    {
        public RemoteUser(PamelloUserDTO userDTO, PamelloClient client) : base(userDTO, client) {

        }

		public string? AvatarUrl {
            get => _dto.AvatarUrl;
        }
		public ulong DiscordId {
            get => _dto.DiscordId;
        }
		public int? SelectedPlayerId {
            get => _dto.SelectedPlayerId;
        }
        public int SongsPlayed {
            get => _dto.SongsPlayed;
        }
		public bool IsAdministrator {
            get => _dto.IsAdministrator;
        }
        public DateTime JoinedAt {
            get => _dto.JoinedAt;
        }

        public IEnumerable<int> AddedSongsIds {
            get => _dto.AddedSongsIds;
        }
        public IEnumerable<int> AddedPlaylistsIds {
            get => _dto.AddedPlaylistsIds;
        }
        public IEnumerable<int> FavoriteSongsIds {
            get => _dto.FavoriteSongsIds;
        }
        public IEnumerable<int> FavoritePlaylistsIds {
            get => _dto.FavoritePlaylistsIds;
        }

        public async Task<RemotePlayer?> GetSelectedPlayer() {
            if (SelectedPlayerId is null) return null;
            return await _client.Players.Get(SelectedPlayerId.Value);
        }

        internal override void FullUpdate(PamelloUserDTO dto) {
            _dto = dto;
        }
    }
}
