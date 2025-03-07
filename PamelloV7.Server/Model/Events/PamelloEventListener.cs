using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events;
using System.Text.Json;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Server.Model.Events
{
    public class PamelloEventListener
    {
        private readonly HttpResponse _response;

        public Guid Token { get; }
        public PamelloUser? User { get; private set; }

        public bool IsAuthorized { get => User is not null; }
        public bool IsClosed { get; private set; }

        public PamelloEventListener(HttpResponse response) {
            _response = response;

            Token = Guid.NewGuid();
        }

        public async Task InitializeConnecion() {
            _response.ContentType = "text/event-stream";
            _response.Headers.CacheControl = "no-cache";
            await _response.Body.FlushAsync();

            await SendEventAsync(new EventsConnected() {
                EventsToken = Token,
            });
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

        public void AssighnUser(PamelloUser user) {
            User = user;
        }

        public void Close() {
            IsClosed = true;
        }
    }
}
