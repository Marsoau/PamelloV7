using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Services;
using PamelloV7.Module.Marsoau.Base.Events.Base;

namespace PamelloV7.Module.Marsoau.Base.Services;

public class EventsService : IEventsService
{
    private readonly ISSEBroadcastService _broadcast;
    
    private List<IEventSubscription> _subscriptions;
    
    public EventsService(IServiceProvider services) {
        _broadcast = services.GetRequiredService<ISSEBroadcastService>();
        
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

        if (eventType.GetCustomAttribute<BroadcastAttribute>() is not null) {
            _broadcast.Broadcast(e);
        }

        foreach (var subscription in _subscriptions) {
            var subscriptionType = subscription.Type;
            
            if (subscriptionType == eventType) {
                subscription.Invoke(e);
            }
        }
    }
}
