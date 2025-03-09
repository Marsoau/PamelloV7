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
        }

        internal async Task<T?> HttpGetAsync<T>(string url) {
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://{ServerHost}/{url}");
            if (UserToken is not null) {
                request.Headers.Add("user", UserToken.Value.ToString());
            }

            var responce = await _http.SendAsync(request);
            if (responce.StatusCode != System.Net.HttpStatusCode.OK) {
                if (responce.Content is null) throw new Exception("Unknown error ocured");
                throw new Exception(await responce.Content.ReadAsStringAsync());
            }

            var result = JsonSerializer.Deserialize<T>(responce.Content.ReadAsStream());

            return result;
        }

        public async Task Connect(string serverHost) {
            await Events.Connect(serverHost);
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
    }
}
