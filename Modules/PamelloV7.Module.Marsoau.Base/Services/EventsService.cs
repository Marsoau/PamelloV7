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
    
    public EventsService(IServiceProvider services) {
        _services = services;
        
        _history = services.GetRequiredService<IHistoryService>();
        
        _sse = services.GetRequiredService<ISSEBroadcastService>();
        _signal = services.GetRequiredService<ISignalBroadcastService>();
        
        _eventSubscriptions = [];
        _updateSubscriptions = [];
    }
    
    public IEventSubscription Subscribe<TEventType>(Func<TEventType, Task> handler) where TEventType : IPamelloEvent {
        return Subscribe<TEventType>((_, e) => handler(e));
    }

    public IEventSubscription Subscribe<TEventType>(Func<IPamelloUser?, TEventType, Task> handler) where TEventType : IPamelloEvent {
        var subscription = new EventSubscription<TEventType>(handler);

        if (subscription is not IEventSubscription castedSubscription) {
            throw new Exception("Failed to cast subscription");
        }
        
        //addition
        _eventSubscriptions.Add(castedSubscription);
        
        return subscription;
    }

    public IEventSubscription Subscribe<TEventType>(Action<TEventType> handler) where TEventType : IPamelloEvent {
        return Subscribe<TEventType>((_, e) => handler(e));
    }

    public IEventSubscription Subscribe<TEventType>(Action<IPamelloUser?, TEventType> handler) where TEventType : IPamelloEvent {
        return Subscribe<TEventType>((user, e) => {
            handler(user, e);
            return Task.CompletedTask;
        });
    }

    public IUpdateSubscription Watch(Func<IPamelloEvent, Task> handler, Func<IPamelloEntity?[]> watchedEntities) {
        var subscription = new UpdateSubscription(handler, watchedEntities);
        
        _updateSubscriptions.Add(subscription);
        
        return subscription;
    }

    public Task<IHistoryRecord?> InvokeAsync<TPamelloEvent>(IPamelloUser? invoker, TPamelloEvent e) where TPamelloEvent : IPamelloEvent {
        return InvokeAsync(typeof(TPamelloEvent), invoker, e);
    }

    public async Task<IHistoryRecord?> InvokeAsync(Type eventType, IPamelloUser? invoker, IPamelloEvent e) {
        var parentEvent = _localEvent.Value;
        _localEvent.Value = e;
        
        try {
            return await InvokeInternal(eventType, invoker, e, parentEvent);
        }
        finally {
            _localEvent.Value = parentEvent;
        }
    }
    public async Task<IHistoryRecord?> InvokeInternal(Type eventType, IPamelloUser? invoker, IPamelloEvent e, IPamelloEvent? parentEvent) {
        Console.WriteLine($"User {invoker?.ToString() ?? "NONE"} invoking event: {eventType.Name}");
        await Task.Run(async () => {
            var tasks = _eventSubscriptions
                .Where(subscription => subscription.EventType == eventType)
                .Select(subscription => subscription.InvokeAsync(invoker, e));
            
            await Task.WhenAll(tasks);
        });

        if (e is RevertiblePamelloEvent revertibleEvent) {
            if (revertibleEvent.RevertPack.GetType().GetField("Services") is { } servicesProperty) {
                servicesProperty.SetValue(revertibleEvent.RevertPack, _services);
            }
            if (revertibleEvent.RevertPack.GetType().GetField("Event") is { } eventProperty) {
                eventProperty.SetValue(revertibleEvent.RevertPack, e);
            }
        }

        IHistoryRecord? record = null;

        if (eventType.GetCustomAttribute<HistoricalEventAttribute>() is not null) {
            if (parentEvent is not null) {
                _history.Record(e, parentEvent, invoker);
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
            await subscription.InvokeAsync(e);
        }

        return record;
    }
}
