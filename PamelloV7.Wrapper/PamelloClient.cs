using PamelloV7.Wrapper.Services;
using PamelloV7.Core.Exceptions;
using System.Text.Json;
using PamelloV7.Wrapper.Repositories;

namespace PamelloV7.Wrapper
{
    public class PamelloClient
    {
        private readonly HttpClient _http;

        public readonly PamelloEventsService Events;
        public readonly PamelloCommandsService Commands;
        public readonly PamelloAuthorizationService Authorization;

        public readonly RemoteUserRepository Users;
        public readonly RemoteSongRepository Songs;
        public readonly RemotePlayerRepository Players;
        public readonly RemoteEpisodeRepository Episodes;
        public readonly RemotePlaylistRepository Playlists;

        public string? ServerHost { get; internal set; }

        public PamelloClient() {
            _http = new HttpClient();

            Events = new PamelloEventsService(this);
            Commands = new PamelloCommandsService(this);
            Authorization = new PamelloAuthorizationService(this);

            Users = new RemoteUserRepository(this);
            Songs = new RemoteSongRepository(this);
            Players = new RemotePlayerRepository(this);
            Episodes = new RemoteEpisodeRepository(this);
            Playlists = new RemotePlaylistRepository(this);
        }

        internal Task HttpGetAsync(string url, Guid? customToken = null)
            => HttpGetAsync<object?>(url, customToken);
        internal async Task<T?> HttpGetAsync<T>(string url, Guid? customToken = null) {
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://{ServerHost}/{url}");
            if (Authorization.UserToken is not null) {
                request.Headers.Add("user", (customToken ?? Authorization.UserToken).Value.ToString());
            }

            var responce = await _http.SendAsync(request);
            var contentString = await responce.Content.ReadAsStringAsync();

            if (responce.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new Exception(contentString);
            }

            if (contentString.Length == 0) return default;

            var result = JsonSerializer.Deserialize<T>(responce.Content.ReadAsStream());

            return result;
        }
    }
}
