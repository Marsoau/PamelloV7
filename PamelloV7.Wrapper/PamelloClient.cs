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

        public Guid? EventsToken { get; internal set; }
        public Guid? UserToken { get; private set; }

        public PamelloClient() {
            _http = new HttpClient();

            _authorization = new PamelloAuthorizationService(this);

            Users = new RemoteUserRepository(this);

            ServerHost = "127.0.0.1:51630";
            UserToken = Guid.Parse("D01E6353-2EC7-469C-81A5-D3084FB17151");
        }

        internal async Task<T?> HttpGetAsync<T>(string url) {
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://{ServerHost}/{url}");
            if (UserToken is not null) {
                request.Headers.Add("user", UserToken.Value.ToString());
            }

            var responce = await _http.SendAsync(request);
            //Console.WriteLine(await responce.Content.ReadAsStringAsync());
            if (responce.StatusCode != System.Net.HttpStatusCode.OK) {
                return default;
            }

            var result = JsonSerializer.Deserialize<T>(responce.Content.ReadAsStream());

            return result;
        }

        public async Task Connect(string serverHost) {

        }

        public async Task Authorize(int code) {
            var token = await HttpGetAsync<Guid>($"Authorization/{EventsToken}/WithCode/{code}");


        }
        public async Task Authorize(Guid userToken) {
            var token = await HttpGetAsync<Guid>($"Authorization/{EventsToken}/WithToken/{userToken}");
        }
    }
}
