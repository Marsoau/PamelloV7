using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PamelloV7.Wrapper.Services
{
    public class PamelloEventsService
    {
        private readonly HttpClient _http;
        private readonly PamelloClient _pamelloClient;
        
        

        public PamelloEventsService() {
            _http = new HttpClient();
        }

        public async Task Connect(string serverHost) {
            await _http.GetStreamAsync(serverHost);
        }
    }
}
