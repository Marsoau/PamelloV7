using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Events.Enumerators;
using PamelloV7.Core.History.Services;
using PamelloV7.Core.Services;
using PamelloV7.Module.Marsoau.Base.Events.Base;

namespace PamelloV7.Module.Marsoau.Base.Services;

public class EventsService : IEventsService
{
    private readonly IServiceProvider _services;
    
    private readonly IHistoryService _history;

    private readonly ISSEBroadcastService _sse;
    private readonly ISignalBroadcastService _signal;
    
    private List<IEventSubscription> _eventSubscriptions;
    private List<IUpdateSubscription> _updateSubscriptions;

    private static AsyncLocal<IPamelloEvent?> _localEvent = new();
    
    public EventsService(IServiceProvider services) {
        _services = services;
        
        _history = services.GetRequiredService<IHistoryService>();
        
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

    public IEventSubscription Subscribe<TEventType>(Func<IPamelloUser, TEventType, Task> handler) where TEventType : IPamelloEvent {
        throw new NotImplementedException();
    }

    public IUpdateSubscription Watch(Func<IPamelloEvent, Task> handler, Func<IPamelloEntity?[]> watchedEntities) {
        var subscription = new UpdateSubscription(handler, watchedEntities);
        
        _updateSubscriptions.Add(subscription);
        
        return subscription;
    }

    public TEventType Invoke<TEventType>(TEventType e) where TEventType : IPamelloEvent  {
        Invoke(typeof(TEventType), e);
        return e;
    }

    public TPamelloEvent Invoke<TPamelloEvent>(IPamelloUser invoker, TPamelloEvent e) where TPamelloEvent : IPamelloEvent {
        throw new NotImplementedException();
    }

    public IPamelloEvent Invoke(Type eventType, IPamelloEvent e) {
        try {
            return InvokeInternal(eventType, e);
        }
        finally {
            _localEvent.Value = null;
        }
    }
    public IPamelloEvent InvokeInternal(Type eventType, IPamelloEvent e) {
        var parentEvent = _localEvent.Value;
        _localEvent.Value = e;
        
        _updateSubscriptions.RemoveAll(subscription => subscription.IsDisposed);
        
        foreach (var subscription in _eventSubscriptions) {
            var subscriptionType = subscription.EventType;
            
            if (subscriptionType == eventType) {
                subscription.Invoke(e);
            }
        }
        
        if (parentEvent is null) _history.Record(e);
        else _history.Record(e, parentEvent);

        if (eventType.GetCustomAttribute<BroadcastAttribute>() is not null) {
            _sse.Broadcast(e);
            _signal.Broadcast(e);
        }

        if (eventType.GetCustomAttribute<PamelloEventCategory>() is not { Category: EEventCategory.InfoUpdate }) return e;
        
        var property = eventType.GetProperties().FirstOrDefault(prop => prop.GetCustomAttribute<InfoUpdatePropertyAttribute>() is not null);
        if (property is null || !property.PropertyType.IsAssignableTo(typeof(IPamelloEntity))) return e;

        if (property.GetValue(e) is not IPamelloEntity entity) return e;
        if (entity is IPamelloPlayer player) {
            if (eventType.GetCustomAttribute<BroadcastToPlayerAttribute>() is not null) {
                _sse.BroadcastToPlayer(e, player);
                _signal.BroadcastToPlayer(e, player);
            }
        }

        foreach (var subscription in _updateSubscriptions.Where(subscription => subscription.WatchedEntities.Invoke().Contains(entity))) {
            subscription.Invoke(e);
        }

        return e;
    }
}
