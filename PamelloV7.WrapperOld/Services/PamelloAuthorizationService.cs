using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PamelloV7.Core.EventsOld;

namespace PamelloV7.WrapperOld.Services
{
    public class PamelloAuthorizationService
    {
        private readonly PamelloClient _client;

        public PamelloAuthorizationService(PamelloClient client) {
            _client = client;

            _client.Events.OnEventsAuthorized += Events_OnEventsAuthorized;
            _client.Events.OnEventsUnAuthorized += Events_OnEventsUnAuthorized;
        }

        private async Task Events_OnEventsAuthorized(EventsAuthorized arg) {

        }
        private async Task Events_OnEventsUnAuthorized(EventsUnAuthorized arg) {

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

            await WithTokenAsync(userToken.Value);

            return true;
        }
        public async Task WithTokenAsync(Guid token) {
            await Unauthorize();

            _client.UserToken = token;
            await _client.Users.UpdateCurrentUser();
            await _client.Events.Authorize();
        }
        public async Task Unauthorize(bool eventsOnly = false) {
            await _client.Events.Unauthorize();

            if (!eventsOnly) _client.UserToken = null;

            await _client.Users.UpdateCurrentUser();
        }
    }
}
