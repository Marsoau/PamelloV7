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

            _client.Events.OnEventsAuthorized += Events_OnEventsAuthorized;
            _client.Events.OnEventsUnAuthorized += Events_OnEventsUnAuthorized;
        }

        private async Task Events_OnEventsAuthorized(Core.Events.EventsAuthorized arg) {

        }
        private async Task Events_OnEventsUnAuthorized(Core.Events.EventsUnAuthorized arg) {

        }

        public async Task<bool> WithCodeAsync(int code, bool authorizeEvents = true) {
            Guid? userToken;

            try {
                userToken = await _client.HttpGetAsync<Guid?>($"Authorization/GetToken/{code}");
            }
            catch {
                userToken = null;
                return false;
            }

            if (userToken is null) return false;

            await WithTokenAsync(userToken.Value, authorizeEvents);

            return true;
        }
        public async Task WithTokenAsync(Guid token, bool authorizeEvents = true) {
            await Unauthorize();

            UserToken = token;
            await _client.Users.UpdateCurrentUser();

            if (authorizeEvents) await _client.Events.Authorize();
        }
        public async Task Unauthorize(bool eventsOnly = false) {
            await _client.Events.UnAuthorize();

            if (!eventsOnly) UserToken = null;

            await _client.Users.UpdateCurrentUser();
        }
    }
}
