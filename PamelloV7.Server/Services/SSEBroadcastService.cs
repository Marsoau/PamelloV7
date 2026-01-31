using PamelloV7.Core.Entities;
using PamelloV7.Server.Model;
using PamelloV7.Core.Enumerators;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.EventsOld;
using PamelloV7.Server.Model.Listeners;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Repositories;
using PamelloV7.Core.Services;

namespace PamelloV7.Server.Services
{
    public class SSEBroadcastService : ISSEBroadcastService
    {
        private readonly ICodeAuthorizationService _authorization;
        private readonly IPamelloUserRepository _users;

        private readonly List<PamelloSSEListener> _listeners;

        public SSEBroadcastService(ICodeAuthorizationService authorization, IPamelloUserRepository users) {
            _authorization = authorization;
            _users = users;

            _listeners = new List<PamelloSSEListener>();

            Task.Run(RemoveListenersThread);
        }

        private async Task RemoveListenersThread() {
            var removeQueue = new Queue<PamelloSSEListener>();

            while (true) {
                foreach (var listener in _listeners) {
                    if (listener.IsClosed) removeQueue.Enqueue(listener);
                }
                while (removeQueue.Count > 0) {
                    await RemoveListener(removeQueue.Dequeue());
                }
                await Task.Delay(1000);
            }
        }

        private async Task RemoveListener(PamelloSSEListener listener) {
            _listeners.Remove(listener);
            await listener.CloseConnection();

            Console.WriteLine($"removed \"{listener.Token}\" events");
        }

        public async Task<PamelloSSEListener> AddListener(HttpResponse response, CancellationToken cancellationToken) {
            var listener = new PamelloSSEListener(response, cancellationToken);
            await listener.InitializeConnection();

            _listeners.Add(listener);
            return listener;
        }

        private PamelloSSEListener? GetListener(Guid eventsToken) {
            return _listeners.FirstOrDefault(listener => listener.Token == eventsToken);
        }
        private PamelloSSEListener GetRequiredListener(Guid eventsToken) {
            return GetListener(eventsToken) ?? throw new PamelloException($"Events with token \"{eventsToken}\" doesnt exist");
        }

        public void Broadcast(PamelloEvent pamelloEvent) {
            throw new NotImplementedException();
            //delete this method later
        }
        public void BroadcastToPlayer(IPamelloPlayer player, PamelloEvent pamelloEvent) {
            throw new NotImplementedException();
            //delete this method later
        }
        
        public void Broadcast(IPamelloEvent pamelloEvent)
        {
            foreach (var listener in _listeners) {
                if (!listener.IsAuthorized) continue;
                listener.ScheduleEvent(pamelloEvent);
            }
        }
        public void BroadcastToPlayer(IPamelloPlayer player, IPamelloEvent pamelloEvent)
        {
            foreach (var listener in _listeners) {
                if (listener.User is null || listener.User.SelectedPlayer?.Id != player?.Id) continue;
                listener.ScheduleEvent(pamelloEvent);
            }
        }
        public void BroadcastToUser(IPamelloUser user, IPamelloEvent pamelloEvent)
        {
            foreach (var listener in _listeners) {
                if (listener.User is null || listener.User.Id != user.Id) continue;
                listener.ScheduleEvent(pamelloEvent);
            }
        }

        public PamelloSSEListener AuthorizeEventsWithCode(Guid eventsToken, int code) {
            var user = _authorization.GetUser(code);
            if (user is null) throw new PamelloException($"User with authorization code \"{code}\" not found");

            return AuthorizeEventsWithToken(eventsToken, user.Token);
        }
        public PamelloSSEListener AuthorizeEventsWithToken(Guid eventsToken, Guid userToken) {
            var events = GetRequiredListener(eventsToken);

            var user = _users.GetByToken(userToken);
            if (user is null) throw new PamelloException($"User with token \"{userToken}\" not found");

            events.AssighnUser(user);

            return events;
        }
        public void UnauthorizeEvents(Guid eventsToken) {
            var events = GetRequiredListener(eventsToken);

            events.AbandonUser();
        }

        public void CloseEvents(Guid eventsToken) {
            var events = GetRequiredListener(eventsToken);

            events.AbandonUser();
            events.Close();
        }

        public void Shutdown() {
            Console.WriteLine("STOPPING SSE");
            foreach (var listener in _listeners) {
                listener.Dispose();
            }
        }
    }
}
