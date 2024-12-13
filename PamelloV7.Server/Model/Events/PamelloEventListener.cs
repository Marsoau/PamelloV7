using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using System.Text.Json;

namespace PamelloV7.Server.Model.Events
{
    public class PamelloEventListener
    {
        public readonly PamelloUser User;
        private readonly HttpResponse _response;

        public bool IsClosed { get; private set; }

        public PamelloEventListener(PamelloUser user, HttpResponse response) {
            User = user;
            _response = response;
        }

        public async Task InitializeConnecion() {
            _response.ContentType = "text/event-stream";
            _response.Headers.CacheControl = "no-cache";
            await _response.Body.FlushAsync();
        }

        public async Task SendEventAsync<TEventType>(TEventType pamelloEvent)
            where TEventType : PamelloEvent
        {
            if (IsClosed) return;

            await _response.WriteAsync($"id: {(int)pamelloEvent.EventName}\revent: {pamelloEvent.EventName}\rdata: {JsonSerializer.Serialize(pamelloEvent)}\r\r");
            await _response.Body.FlushAsync();
        }

        public void SendEvent<TEventType>(TEventType pamelloEvent)
            where TEventType : PamelloEvent
            => Task.Run(() => SendEventAsync(pamelloEvent));

        public void Close() {
            IsClosed = true;
        }
    }
}
