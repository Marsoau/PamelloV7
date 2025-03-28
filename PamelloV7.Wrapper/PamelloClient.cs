using PamelloV7.Wrapper.Services;
using PamelloV7.Core.Exceptions;
using System.Text.Json;
using PamelloV7.Wrapper.Repositories;

namespace PamelloV7.Wrapper
{
    public class PamelloClient
    {
        private readonly HttpClient _http;

        private readonly PamelloAuthorizationService _authorization;

        public readonly PamelloEventsService Events;
        public readonly PamelloCommandsService Commands;

        public readonly RemoteUserRepository Users;
        public readonly RemoteSongRepository Songs;
        public readonly RemotePlayerRepository Players;
        public readonly RemoteEpisodeRepository Episodes;
        public readonly RemotePlaylistRepository Playlists;

        public string? ServerHost { get; internal set; }

        public Guid? EventsToken { get; internal set; }
        public Guid? UserToken { get; private set; }

        public event Func<Task>? OnAuthorized;

        public PamelloClient() {
            _http = new HttpClient();

            _authorization = new PamelloAuthorizationService(this);

            Events = new PamelloEventsService(this);
            Commands = new PamelloCommandsService(this);

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
            if (UserToken is not null) {
                request.Headers.Add("user", (customToken ?? UserToken).Value.ToString());
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

        public async Task Connect(string serverHost) {
            await Events.Connect(serverHost);
        }
        public async Task<bool> TryConnect(string serverHost) {
            try {
                await Events.Connect(serverHost);
            }
            catch {
                return false;
            }

            return true;
        }

        public async Task<bool> TryAuthorize(int code) {
            try {
                await Authorize(code);
            }
            catch {
                return false;
            }

            return true;
        }
        public async Task<bool> TryAuthorize(Guid userToken) {
            try {
                await Authorize(userToken);
            }
            catch {
                return false;
            }

            return true;
        }

        public async Task Authorize(int code) {
            if (EventsToken is null) throw new Exception("You have to connect to the server first");

            var token = await HttpGetAsync<Guid?>($"Authorization/{EventsToken}/WithCode/{code}");

            UserToken = token;
            await Users.UpdateCurrentUser();

            OnAuthorized?.Invoke();
        }
        public async Task Authorize(Guid userToken) {
            if (EventsToken is null) throw new Exception("You have to connect to the server first");

            var token = await HttpGetAsync<Guid?>($"Authorization/{EventsToken}/WithToken/{userToken}");

            UserToken = token;
            await Users.UpdateCurrentUser();

            OnAuthorized?.Invoke();
        }

        public async Task Unauthorize() {
            if (EventsToken is null) throw new Exception("You have to be connected to the server");
            if (UserToken is null) throw new Exception("You have to be authorized");

            await HttpGetAsync($"Authorization/Close/{EventsToken}/{UserToken}");
            UserToken = null;
        }
    }
}
