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

        public readonly RemoteUserRepository Users;

        private string? ServerHost;

        public Guid? CurrentToken { get; set; }

        public PamelloClient() {
            _http = new HttpClient();

            _authorization = new PamelloAuthorizationService(this);

            Users = new RemoteUserRepository(this);

            ServerHost = "127.0.0.1:51630";
            CurrentToken = Guid.Parse("D01E6353-2EC7-469C-81A5-D3084FB17151");
        }

        internal async Task<T?> HttpGetAsync<T>(string url) {
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://{ServerHost}/{url}");
            if (CurrentToken is not null) {
                request.Headers.Add("user", CurrentToken.Value.ToString());
            }

            var responce = await _http.SendAsync(request);
            //Console.WriteLine(await responce.Content.ReadAsStringAsync());
            if (responce.StatusCode != System.Net.HttpStatusCode.OK) {
                return default;
            }

            var result = JsonSerializer.Deserialize<T>(responce.Content.ReadAsStream());

            return result;
        }

        public async Task ConnectWithCodeAsync(int code) {
            var token = await _authorization.GetTokenWithCodeAsync(code);
            if (token is null) throw new PamelloException($"cant get token with code \"{code}\"");

            await ConnectWithTokenAsync(token.Value);
        }
        public async Task ConnectWithTokenAsync(Guid token) {
            //if (await _events.TryConnectAsync(token)) throw new PamelloException($"cant connect to the events with token \"{token}\"");

            CurrentToken = token;
        }
    }
}
