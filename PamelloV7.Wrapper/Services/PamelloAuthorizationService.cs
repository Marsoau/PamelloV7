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

        public PamelloAuthorizationService(PamelloClient client) {
            _client = client;
        }

        public async Task<Guid?> GetTokenWithCodeAsync(int code) {
            try {
                return await _client.HttpGetAsync<Guid?>($"Authorization/WithCode/{code}");
            }
            catch {
                return null;
            }
        }
    }
}
