using PamelloV7.Core.DTO;
using PamelloV7.Wrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Repositories
{
    public class RemoteUserRepository : RemoteRepository<RemoteUser, PamelloUserDTO>
    {
        private RemoteUser? _current;

        public RemoteUser Current {
            get {
                if (_current is not null) return _current;
                UpdateCurrentUser().Wait();
                
                return _current;
            }
        }

        protected override string ControllerName => "User";

        public RemoteUserRepository(PamelloClient client) : base(client) {
            _current = null;

            SubscribeToEventsDataUpdates();
        }

        internal void SubscribeToEventsDataUpdates() {
            _client.Events.OnUserAddedSongsUpdated += async (e) => {
                var user = await Get(e.UserId, false);
                if (user is not null) user._dto.AddedSongsIds = e.AddedSongsIds;
            };
            _client.Events.OnUserAvatarUpdated += async (e) => {
                var user = await Get(e.UserId, false);
                if (user is not null) user._dto.AvatarUrl = e.AvatarUrl;
            };
            _client.Events.OnUserFavoritePlaylistsUpdated += async (e) => {
                var user = await Get(e.UserId, false);
                if (user is not null) user._dto.FavoritePlaylistsIds = e.FavoritePlaylistsIds;
            };
            _client.Events.OnUserFavoriteSongsUpdated += async (e) => {
                var user = await Get(e.UserId, false);
                if (user is not null) user._dto.FavoriteSongsIds = e.FavoriteSongsIds;
            };
            _client.Events.OnUserIsAdministratorUpdated += async (e) => {
                var user = await Get(e.UserId, false);
                if (user is not null) user._dto.IsAdministrator = e.IsAdministrator;
            };
            _client.Events.OnUserNameUpdated += async (e) => {
                var user = await Get(e.UserId, false);
                if (user is not null) user._dto.Name = e.Name;
            };
            _client.Events.OnUserSongsPlayedUpdated += async (e) => {
                var user = await Get(e.UserId, false);
                if (user is not null) user._dto.SongsPlayed = e.SongsPlayed;
            };
            _client.Events.OnUserSelectedPlayerIdUpdated += async (e) => {
                var user = await Get(e.UserId, false);
                if (user is not null) user._dto.SelectedPlayerId = e.SelectedPlayerId;
            };
            _client.Events.OnUserAddedPlaylistsUpdated += async (e) => {
                var user = await Get(e.UserId, false);
                if (user is not null) user._dto.AddedPlaylistsIds = e.AddedPlaylistsIds;
            };
        }

        internal async Task UpdateCurrentUser() {
            if (_client.UserToken is null) {
                _current = null;
            }
            else {
                _current = await GetNew(_client.UserToken.Value);
            }
        }

        public async Task<RemoteUser?> GetNew(Guid token) {
            return GetFromDTO(await GetDTO(token));
        }

        protected override Task<PamelloUserDTO?> GetDTO(string value)
            => _client.HttpGetAsync<PamelloUserDTO>($"Data/User/{value}");
        protected Task<PamelloUserDTO?> GetDTO(Guid token)
            => _client.HttpGetAsync<PamelloUserDTO>($"Data/User/current", token);
        protected override RemoteUser CreateRemoteEntity(PamelloUserDTO dto)
            => new RemoteUser(dto, _client);
    }
}
