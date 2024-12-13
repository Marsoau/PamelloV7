using PamelloV7.Server.Model;
using PamelloV7.Core.Enumerators;
using PamelloV7.Server.Model.Events;
using PamelloV7.Server.Model.Audio;
using PamelloV7.Core.Events;

namespace PamelloV7.Server.Services
{
    public class PamelloEventsService
    {
        private readonly List<PamelloEventListener> _listeners;

        public PamelloEventsService() {
            _listeners = new List<PamelloEventListener>();
        }

        public async Task<PamelloEventListener> AddUserListener(PamelloUser user, HttpResponse response) {
            var listener = new PamelloEventListener(user, response);
            await listener.InitializeConnecion();

            _listeners.Add(listener);
            return listener;
        }

        public async Task BroadcastAsync<TEventType>(TEventType pamelloEvent)
            where TEventType : PamelloEvent
        {
            foreach (var listener in _listeners) {
                await listener.SendEventAsync(pamelloEvent);
            }
        }
        public async Task BroadcastToPlayerAsync<TEventType>(PamelloPlayer player, TEventType pamelloEvent)
            where TEventType : PamelloEvent
        {
            foreach (var listener in _listeners) {
                if (listener.User.SelectedPlayer?.Id != player?.Id) continue;
                await listener.SendEventAsync(pamelloEvent);
            }
        }
        public async Task BroadcastToUserAsync<TEventType>(PamelloUser user, TEventType pamelloEvent)
            where TEventType : PamelloEvent
        {
            foreach (var listener in _listeners) {
                if (listener.User.Id != user.Id) continue;
                await listener.SendEventAsync(pamelloEvent);
            }
        }

        public void Broadcast<TEventType>(TEventType pamelloEvent)
            where TEventType : PamelloEvent
            => Task.Run(() => BroadcastAsync(pamelloEvent));
        public void BroadcastToPlayer<TEventType>(PamelloPlayer player, TEventType pamelloEvent)
            where TEventType : PamelloEvent
            => Task.Run(() => BroadcastToPlayerAsync(player, pamelloEvent));
        public void BroadcastToUser<TEventType>(PamelloUser user, TEventType pamelloEvent)
            where TEventType : PamelloEvent
            => Task.Run(() => BroadcastToUserAsync(user, pamelloEvent));
    }
}
