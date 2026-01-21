using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Services;
using PamelloV7.Module.Marsoau.Base.Events.Base;

namespace PamelloV7.Module.Marsoau.Base.Services;

public class EventsService : IEventsService
{
    private readonly ISSEBroadcastService _broadcast;
    
    private readonly IPamelloLogger _logger;
    
    private List<IEventSubscription> _eventSubscriptions;
    private List<IUpdateSubscription> _updateSubscriptions;
    
    public EventsService(IServiceProvider services) {
        _broadcast = services.GetRequiredService<ISSEBroadcastService>();
        
        _logger = services.GetRequiredService<IPamelloLogger>();
        
        _eventSubscriptions = [];
        _updateSubscriptions = [];
    }
    
    public IEventSubscription Subscribe<TEventType>(Func<TEventType, Task> handler) where TEventType : IPamelloEvent {
        var subscription = new EventSubscription<TEventType>(handler);

        var castedSubscription = subscription as IEventSubscription;
        if (castedSubscription is null) {
            throw new Exception("Failed to cast subscription");
        }
        
        _eventSubscriptions.Add(castedSubscription);
        
        return subscription;
    }

    public IUpdateSubscription Watch(Func<IPamelloEvent, Task> handler, Func<IPamelloEntity[]> watchedEntities) {
        var subscription = new UpdateSubscription(handler, watchedEntities);
        
        _updateSubscriptions.Add(subscription);
        
        return subscription;
    }

    public void Invoke<TEventType>(TEventType e) where TEventType : IPamelloEvent {
        var eventType = e.GetType();

        _logger.Log($"Event: {eventType}");
        
        _updateSubscriptions.RemoveAll(subscription => subscription.IsDisposed);

        Console.WriteLine($"{_updateSubscriptions.Count} WATCHERS");
        
        foreach (var subscription in _eventSubscriptions) {
            var subscriptionType = subscription.EventType;
            
            if (subscriptionType == eventType) {
                subscription.Invoke(e);
            }
        }

        if (eventType.GetCustomAttribute<BroadcastAttribute>() is not null) {
            _broadcast.Broadcast(e);
        }

        if (eventType.GetCustomAttribute<InfoUpdateAttribute>() is not { } eventAttribute) return;
        
        var property = eventType.GetProperties().FirstOrDefault(prop => prop.GetCustomAttribute<InfoUpdatePropertyAttribute>() is not null);
        if (property is null || !property.PropertyType.IsAssignableTo(typeof(IPamelloEntity))) return;

        if (property.GetValue(e) is not IPamelloEntity entity) return;

        foreach (var subscription in _updateSubscriptions.Where(subscription => subscription.WatchedEntities.Invoke().Contains(entity))) {
            subscription.Invoke(e);
        }
    }
}
