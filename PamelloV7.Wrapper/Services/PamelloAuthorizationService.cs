using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Services
{
    public class PamelloAuthorizationService
    {
        private readonly PamelloClient _client;

        public Guid? UserToken { get; internal set; }

        public PamelloAuthorizationService(PamelloClient client) {
            _client = client;
        }

        public async Task<bool> WithCodeAsync(int code) {
            Guid? userToken;

            try {
                userToken = await _client.HttpGetAsync<Guid?>($"Authorization/GetToken/{code}");
            }
            catch {
                userToken = null;
                return false;
            }

            if (userToken is null) return false;

            await WithTokenAsync(userToken.Value);

            return true;
        }
        public async Task WithTokenAsync(Guid token) {
            await Unauthorize();

            UserToken = token;

            await _client.Events.Authorize();
        }
        public async Task Unauthorize(bool eventsOnly = false) {
            await _client.Events.UnAuthorize();

            if (!eventsOnly) UserToken = null;
        }
    }
}
