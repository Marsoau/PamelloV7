using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Services;
using PamelloV7.Plugin.Base.Events.Base;

namespace PamelloV7.Plugin.Base.Services;

public class EventsService : IEventsService
{
    private List<IEventSubscription> _subscriptions;
    
    public EventsService() {
        _subscriptions = new List<IEventSubscription>();
    }
    
    public IEventSubscription Subscribe<TEventType>(Func<TEventType, Task> handler) where TEventType : IPamelloEvent {
        var subscription = new EventSubscription<TEventType>(handler);

        var castedSubscription = subscription as IEventSubscription;
        if (castedSubscription is null) {
            throw new Exception("Failed to cast subscription");
        }
        
        _subscriptions.Add(castedSubscription);
        
        return subscription;
    }

    public void Invoke<TEventType>(TEventType e) where TEventType : IPamelloEvent {
        var eventType = e.GetType();

        foreach (var subscription in _subscriptions) {
            var subscriptionType = subscription.Type;
            
            if (subscriptionType == eventType) {
                subscription.Invoke(e);
            }
        }
    }
}
