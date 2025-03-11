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
        public RemoteUser Current { get; protected set; }

        protected override string ControllerName => "User";

        public RemoteUserRepository(PamelloClient client) : base(client) {
            Current = null;

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
            if (_client.UserToken is null)
                throw new Exception("Cant update current user, token is null");

            Current = await GetNew(_client.UserToken.Value) ?? throw new Exception($"Cant update current user by token \"{_client.UserToken}\"");
        }

        public async Task<RemoteUser?> GetNew(Guid token) {
            return GetFromDTO(await GetDTO(token));
        }

        protected override Task<PamelloUserDTO?> GetDTO(int id)
            => _client.HttpGetAsync<PamelloUserDTO>($"Data/User?id={id}");
        protected override Task<PamelloUserDTO?> GetDTO(string value)
            => _client.HttpGetAsync<PamelloUserDTO>($"Data/User?value={value}");
        protected Task<PamelloUserDTO?> GetDTO(Guid token)
            => _client.HttpGetAsync<PamelloUserDTO>($"Data/User?token={token}");
        protected override RemoteUser CreateRemoteEntity(PamelloUserDTO dto)
            => new RemoteUser(dto, _client);
    }
}
