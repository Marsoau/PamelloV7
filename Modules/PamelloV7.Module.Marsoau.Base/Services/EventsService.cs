using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Events.Enumerators;
using PamelloV7.Core.Events.RestorePacks.Base;
using PamelloV7.Core.History.Records;
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
    
    private readonly List<IEventSubscription> _eventSubscriptions;
    private readonly List<IUpdateSubscription> _updateSubscriptions;

    private static AsyncLocal<IPamelloEvent?> _localEvent = new();
    private static AsyncLocal<List<IEventSubscription>> _localScopedSubscriptions = new();
    
    public EventsService(IServiceProvider services) {
        _services = services;
        
        _history = services.GetRequiredService<IHistoryService>();
        
        _sse = services.GetRequiredService<ISSEBroadcastService>();
        _signal = services.GetRequiredService<ISignalBroadcastService>();
        
        _eventSubscriptions = [];
        _updateSubscriptions = [];
    }

    public IEventSubscription Subscribe<TEventType>(Action<TEventType> handler) where TEventType : IPamelloEvent {
        return Subscribe<TEventType>((_, e) => handler(e));
    }

    public IEventSubscription Subscribe<TEventType>(Action<IPamelloUser?, TEventType> handler) where TEventType : IPamelloEvent {
        var subscription = new EventSubscription<TEventType>(handler);

        if (subscription is not IEventSubscription castedSubscription) {
            throw new Exception("Failed to cast subscription");
        }
        
        //addition
        _eventSubscriptions.Add(castedSubscription);
        
        return subscription;
    }

    public IUpdateSubscription Watch(Func<IPamelloEvent, Task> handler, Func<IPamelloEntity?[]> watchedEntities) {
        var subscription = new UpdateSubscription(handler, watchedEntities);
        
        _updateSubscriptions.Add(subscription);
        
        return subscription;
    }

    public HistoryRecord? Invoke(IPamelloUser? invoker, IPamelloEvent e) {
        return Invoke(invoker, e, null);
    }
    
    public HistoryRecord? Invoke(IPamelloUser? invoker, IPamelloEvent e, Action? action) {
        var parentEvent = _localEvent.Value;
        _localEvent.Value = e;
        
        try {
            return InvokeInternal(e, parentEvent, invoker, action);
        }
        finally {
            _localEvent.Value = parentEvent;
        }
    }
    
    public HistoryRecord? InvokeInternal(IPamelloEvent e, IPamelloEvent? parentEvent, IPamelloUser? invoker, Action? additionalAction) {
        var eventType = e.GetType();
        
        Console.WriteLine($"User {invoker?.ToString() ?? "NONE"} invoking event: {eventType.Name}");
        foreach (var subscription in _eventSubscriptions.Where(subscription => subscription.EventType == eventType)) {
            subscription.Invoke(invoker, e);
        }

        additionalAction?.Invoke();
        
        if (e is RevertiblePamelloEvent { RevertPack.IsActivated: false } revertibleEvent) {
            if (revertibleEvent.RevertPack.GetType().GetField("Services") is { } servicesProperty) {
                servicesProperty.SetValue(revertibleEvent.RevertPack, _services);
            }
            if (revertibleEvent.RevertPack.GetType().GetField("Event") is { } eventProperty) {
                eventProperty.SetValue(revertibleEvent.RevertPack, e);
            }
        }

        HistoryRecord? record = null;

        if (eventType.GetCustomAttribute<HistoricalEventAttribute>() is not null) {
            if (parentEvent is not null) {
                _history.Record(e, parentEvent);
            }
            else {
                record = _history.Record(e, invoker);
            }
        }

        if (eventType.GetCustomAttribute<BroadcastAttribute>() is not null) {
            _sse.Broadcast(e);
            _signal.Broadcast(e);
        }

        if (eventType.GetCustomAttribute<PamelloEventCategory>() is not { Category: EEventCategory.InfoUpdate }) return record;
        
        var property = eventType.GetProperties().FirstOrDefault(prop => prop.GetCustomAttribute<InfoUpdatePropertyAttribute>() is not null);
        if (property is null || !property.PropertyType.IsAssignableTo(typeof(IPamelloEntity))) return record;

        if (property.GetValue(e) is not IPamelloEntity entity) return record;
        if (entity is IPamelloPlayer player) {
            if (eventType.GetCustomAttribute<BroadcastToPlayerAttribute>() is not null) {
                _sse.BroadcastToPlayer(e, player);
                _signal.BroadcastToPlayer(e, player);
            }
        }
        
        _updateSubscriptions.RemoveAll(subscription => subscription.IsDisposed);

        foreach (var subscription in _updateSubscriptions.Where(subscription => subscription.WatchedEntities.Invoke().Contains(entity))) {
            subscription.InvokeAsync(e);
        }
        
        return record;
    }
}
