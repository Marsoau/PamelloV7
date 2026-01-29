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
    private readonly IServiceProvider _services;
    
    private readonly IPamelloLogger _logger;

    private readonly ISSEBroadcastService _sse;
    private readonly ISignalBroadcastService _signal;
    
    private List<IEventSubscription> _eventSubscriptions;
    private List<IUpdateSubscription> _updateSubscriptions;
    
    public EventsService(IServiceProvider services) {
        _services = services;
        
        _logger = services.GetRequiredService<IPamelloLogger>();
        
        _sse = services.GetRequiredService<ISSEBroadcastService>();
        _signal = services.GetRequiredService<ISignalBroadcastService>();
        
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

    public TEventType Invoke<TEventType>(TEventType e) where TEventType : IPamelloEvent  {
        Invoke(typeof(TEventType), e);
        return e;
    }

    public IPamelloEvent Invoke(Type eventType, IPamelloEvent e) {
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
            _sse.Broadcast(e);
            _signal.Broadcast(e);
        }

        if (eventType.GetCustomAttribute<InfoUpdateAttribute>() is null) return e;
        
        var property = eventType.GetProperties().FirstOrDefault(prop => prop.GetCustomAttribute<InfoUpdatePropertyAttribute>() is not null);
        if (property is null || !property.PropertyType.IsAssignableTo(typeof(IPamelloEntity))) return e;

        if (property.GetValue(e) is not IPamelloEntity entity) return e;

        foreach (var subscription in _updateSubscriptions.Where(subscription => subscription.WatchedEntities.Invoke().Contains(entity))) {
            subscription.Invoke(e);
        }

        return e;
    }
}
