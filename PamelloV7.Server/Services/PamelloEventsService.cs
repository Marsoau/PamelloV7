using PamelloV7.Server.Model;
using PamelloV7.Core.Enumerators;
using PamelloV7.Server.Model.Listeners;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Core.Events;
using PamelloV7.Core.Exceptions;
using PamelloV7.Server.Repositories;

namespace PamelloV7.Server.Services
{
    public class PamelloEventsService
    {
        private readonly UserAuthorizationService _userAuthorization;
        private readonly PamelloUserRepository _users;

        private readonly List<PamelloEventListener> _listeners;

        public PamelloEventsService(UserAuthorizationService userAuthorization, PamelloUserRepository users) {
            _userAuthorization = userAuthorization;
            _users = users;

            _listeners = new List<PamelloEventListener>();

            Task.Run(RemoveListenersThread);
        }

        private async Task RemoveListenersThread() {
            var removeQueue = new Queue<PamelloEventListener>();

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

        private async Task RemoveListener(PamelloEventListener listener) {
            _listeners.Remove(listener);
            await listener.CloseConnection();

            Console.WriteLine($"removed \"{listener.Token}\" events");
        }

        public async Task<PamelloEventListener> AddListener(HttpResponse response) {
            var listener = new PamelloEventListener(response);
            await listener.InitializeConnecion();

            _listeners.Add(listener);
            return listener;
        }

        private PamelloEventListener? GetListener(Guid eventsToken) {
            return _listeners.FirstOrDefault(listener => listener.Token == eventsToken);
        }
        private PamelloEventListener GetRequiredListener(Guid eventsToken) {
            return GetListener(eventsToken) ?? throw new PamelloException($"Events with token \"{eventsToken}\" doesnt exist");
        }

        public void Broadcast<TEventType>(TEventType pamelloEvent)
            where TEventType : PamelloEvent
        {
            foreach (var listener in _listeners) {
                if (listener.User is null) continue;
                listener.ScheduleEvent(pamelloEvent);
            }
        }
        public void BroadcastToPlayer<TEventType>(PamelloPlayer player, TEventType pamelloEvent)
            where TEventType : PamelloEvent
        {
            foreach (var listener in _listeners) {
                if (listener.User is null || listener.User.SelectedPlayer?.Id != player?.Id) continue;
                listener.ScheduleEvent(pamelloEvent);
            }
        }
        public void BroadcastToUser<TEventType>(PamelloUser user, TEventType pamelloEvent)
            where TEventType : PamelloEvent
        {
            foreach (var listener in _listeners) {
                if (listener.User is null || listener.User.Id != user.Id) continue;
                listener.ScheduleEvent(pamelloEvent);
            }
        }

        public PamelloEventListener AuthorizeEventsWithCode(Guid eventsToken, int code) {
            var userDiscordId = _userAuthorization.GetDiscordId(code);
            if (userDiscordId is null) throw new PamelloException($"Code \"{code}\" is invalid");

            var user = _users.GetByDiscord(userDiscordId.Value);
            if (user is null) throw new PamelloException($"User with discord id \"{userDiscordId}\" not found");

            return AuthorizeEventsWithToken(eventsToken, user.Token);
        }
        public PamelloEventListener AuthorizeEventsWithToken(Guid eventsToken, Guid userToken) {
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
    }
}
