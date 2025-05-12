using PamelloV7.Core.DTO;
using PamelloV7.Wrapper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Repositories
{
    public class RemoteSongRepository : RemoteRepository<RemoteSong, PamelloSongDTO>
    {
        protected override string ControllerName => "Song";

        public RemoteSongRepository(PamelloClient client) : base(client) {
            SubscribeToEventsDataUpdates();
        }

        internal void SubscribeToEventsDataUpdates() {
            _client.Events.OnSongDownloadStarted += async (e) => {
                var song = await Get(e.SongId, false);
                if (song is not null) song._dto.IsDownloading = true;
            };
            _client.Events.OnSongDownloadProgeressUpdated += async (e) => {
                var song = await Get(e.SongId, false);
                if (song is not null) song._dto.DownloadProgress = e.Progress;
            };
            _client.Events.OnSongDownloadFinished += async (e) => {
                var song = await Get(e.SongId, false);
                if (song is not null) song._dto.IsDownloading = false;
            };

            _client.Events.OnSongEpisodesIdsUpdated += async (e) => {
                var song = await Get(e.SongId, false);
                if (song is not null) song._dto.EpisodesIds = e.EpisodesIds;
            };
            _client.Events.OnSongFavoriteByIdsUpdated += async (e) => {
                var song = await Get(e.SongId, false);
                if (song is not null) song._dto.FavoriteByIds = e.FavoriteByIds;
            };
            _client.Events.OnSongNameUpdated += async (e) => {
                var song = await Get(e.SongId, false);
                if (song is not null) song._dto.Name = e.Name;
            };
            _client.Events.OnSongPlayCountUpdated += async (e) => {
                var song = await Get(e.SongId, false);
                if (song is not null) song._dto.PlayCount = e.PlayCount;
            };
            _client.Events.OnSongPlaylistsIdsUpdated += async (e) => {
                var song = await Get(e.SongId, false);
                if (song is not null) song._dto.PlaylistsIds = e.PlaylistsIds;
            };
            _client.Events.OnSongCoverUrlUpdated += async (e) => {
                var song = await Get(e.SongId, false);
                if (song is not null) song._dto.CoverUrl = e.CoverUrl;
            };
            _client.Events.OnSongAssociacionsUpdated += async (e) => {
                var song = await Get(e.SongId, false);
                if (song is not null) song._dto.Associations = e.Associacions;
            };
        }

        public async Task<IEnumerable<int>> Search(string querry = "", string? addedBy = null, string? favoriteBy = null) {
            var atributes = new Dictionary<string, string>();

            if (addedBy is not null) atributes.Add("addedBy", addedBy);
            if (favoriteBy is not null) atributes.Add("favoriteBy", favoriteBy);

            return await GetSearch(querry, atributes);
        }

        protected override Task<PamelloSongDTO?> GetDTO(int id)
            => _client.HttpGetAsync<PamelloSongDTO>($"Data/Song?id={id}");
        protected override Task<PamelloSongDTO?> GetDTO(string value)
            => _client.HttpGetAsync<PamelloSongDTO>($"Data/Song?value={value}");
        protected override RemoteSong CreateRemoteEntity(PamelloSongDTO dto)
            => new RemoteSong(dto, _client);
    }
}
