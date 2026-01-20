using PamelloV7.Core.Enumerators;
using System.Text.Json;
using PamelloV7.Core.Converters;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Events;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Server.Model.Listeners
{
    public class PamelloSSEListener : PamelloListener
    {
        public Guid Token { get; }
        public IPamelloUser? User { get; private set; }

        public bool IsAuthorized => User is not null;

        public readonly Queue<IPamelloEvent> _eventsQueue;

        private readonly AutoResetEvent _eventsWait;

        public TaskCompletionSource Lifetime;

        public PamelloSSEListener(HttpResponse response, CancellationToken cancellationToken) : base(response) {
            Token = Guid.NewGuid();

            _eventsQueue = new Queue<IPamelloEvent>();

            _eventsWait = new AutoResetEvent(false);
            
            cancellationToken.Register(Close);
            
            Lifetime = new TaskCompletionSource();

            Task.Run(EventsSendingThread);
        }

        private async Task EventsSendingThread() {
            while (true) {
                _eventsWait.WaitOne();
                await SendAllEventsAsync();
                _eventsWait.Reset();
            }
        }

        private async Task SendAllEventsAsync() {
            IPamelloEvent pamelloEvent;

            while (_eventsQueue.Count > 0) {
                pamelloEvent = _eventsQueue.Dequeue();

                Console.WriteLine($"Sending: {pamelloEvent.GetType().Name}");

                await _response.WriteAsync($"event: {pamelloEvent.GetType().Name}\n");
                await _response.WriteAsync($"data: {JsonSerializer.Serialize(pamelloEvent, pamelloEvent.GetType(), JsonEntitiesFactory.Options)}\n\n");

                await _response.Body.FlushAsync();
            }
        }

        public override async Task InitializeConnection() {
            _response.ContentType = "text/event-stream";
            _response.Headers.CacheControl = "no-cache";
            await _response.Body.FlushAsync();

            ScheduleEvent(new EventsConnected {
                EventsToken = Token,
            });
        }

        protected override async Task CloseConnectionBase() {
            IsClosed = true;
            try {
                await _response.CompleteAsync();
            }
            catch (ObjectDisposedException x) {

            }
        }

        private static int eventN = 0;
        public void ScheduleEvent(IPamelloEvent pamelloEvent)
        {
            if (IsClosed) return;

            _eventsQueue.Enqueue(pamelloEvent);
            _eventsWait.Set();
        }

        public void AssighnUser(IPamelloUser user) {
            if (User is not null) throw new PamelloException("User is already assighned for that events");

            User = user;

            ScheduleEvent(new EventsAuthorized() {
                UserToken = User.Token,
            });
        }
        public void AbandonUser() {
            if (User is null) return;

            User = null;

            //ScheduleEvent(new EventsUnAuthorized());
        }

        public void Close() {
            if (IsClosed) return;
            IsClosed = true;
            
            Lifetime.SetResult();
        }

        public override void Dispose() {
            Close();
        }
    }
}
