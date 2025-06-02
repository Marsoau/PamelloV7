using PamelloV7.Core.DTO;
using PamelloV7.Wrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Repositories
{
    public class RemotePlaylistRepository : RemoteRepository<RemotePlaylist, PamelloPlaylistDTO>
    {
        protected override string ControllerName => "Playlist";

        public RemotePlaylistRepository(PamelloClient client) : base(client) {
            SubscribeToEventsDataUpdates();
        }

        internal void SubscribeToEventsDataUpdates() {
            _client.Events.OnPlaylistFavoriteByIdsUpdated += async (e) => {
                var playlist = await Get(e.PlaylistId, false);
                if (playlist is not null) playlist._dto.FavoriteByIds = e.FavoriteByIds;
            };
            _client.Events.OnPlaylistNameUpdated += async (e) => {
                var playlist = await Get(e.PlaylistId, false);
                if (playlist is not null) playlist._dto.Name = e.Name;
            };
            _client.Events.OnPlaylistProtectionUpdated += async (e) => {
                var playlist = await Get(e.PlaylistId, false);
                if (playlist is not null) playlist._dto.IsProtected = e.IsProtected;
            };
            _client.Events.OnPlaylistSongsUpdated += async (e) => {
                var playlist = await Get(e.PlaylistId, false);
                if (playlist is not null) playlist._dto.SongsIds = e.SongsIds;
            };
        }

        protected override RemotePlaylist CreateRemoteEntity(PamelloPlaylistDTO dto)
            => new RemotePlaylist(dto, _client);
    }
}
