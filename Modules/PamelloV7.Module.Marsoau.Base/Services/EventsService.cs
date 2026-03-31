using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;
using PamelloV7.Framework.Events.RestorePacks.Base;
using PamelloV7.Framework.History.Records;
using PamelloV7.Framework.History.Services;
using PamelloV7.Framework.Services;
using PamelloV7.Module.Marsoau.Base.Events.Base;

namespace PamelloV7.Module.Marsoau.Base.Services;

public class EventsService : IEventsService
{
    private readonly IServiceProvider _services;
    
    private readonly IHistoryService _history;

    private readonly ISignalBroadcastService _signal;
    
    private readonly List<IEventSubscription> _eventSubscriptions;
    private readonly List<IUpdateSubscription> _updateSubscriptions;

    private static AsyncLocal<IPamelloEvent?> _localEvent = new();
    
    public EventsService(IServiceProvider services) {
        _services = services;
        
        _history = services.GetRequiredService<IHistoryService>();
        
        _signal = services.GetRequiredService<ISignalBroadcastService>();
        
        _eventSubscriptions = [];
        _updateSubscriptions = [];
    }

    public IEventSubscription Subscribe<TEventType>(Action<TEventType> handler) where TEventType : IPamelloEvent {
        var subscription = new EventSubscription<TEventType>(handler);

        if (subscription is not IEventSubscription castedSubscription) {
            throw new Exception("Failed to cast subscription");
        }
        
        _eventSubscriptions.Add(castedSubscription);
        
        return subscription;
    }

    public IUpdateSubscription Watch(Func<IPamelloEvent, Task> handler, Func<IPamelloEntity?[]> watchedEntities) {
        var subscription = new UpdateSubscription(handler, watchedEntities);
        
        _updateSubscriptions.Add(subscription);
        
        return subscription;
    }

    public IHistoryRecord? Invoke(IPamelloUser? invoker, IPamelloEvent e) {
        return Invoke(invoker, e, null);
    }
    
    public IHistoryRecord? Invoke(IPamelloUser? invoker, IPamelloEvent e, Action? action) {
        var parentEvent = _localEvent.Value;
        _localEvent.Value = e;
        
        try {
            return InvokeInternal(e, parentEvent, invoker, action);
        }
        finally {
            _localEvent.Value = parentEvent;
        }
    }
    
    public IHistoryRecord? InvokeInternal(IPamelloEvent e, IPamelloEvent? parentEvent, IPamelloUser? invoker, Action? additionalAction) {
        var eventType = e.GetType();
        
        Debug.WriteLine($"User {invoker?.ToString() ?? "NONE"} invoking event: {eventType.Name}");
        
        e.Invoker = invoker;
        
        foreach (var subscription in _eventSubscriptions.Where(subscription => subscription.EventType.IsAssignableFrom(eventType))) {
            subscription.Invoke(e);
        }

        additionalAction?.Invoke();
        
        if (e is IRevertiblePamelloEvent { RevertPack.IsActivated: false } revertibleEvent) {
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
                _history.Record(e, parentEvent);
            }
            else {
                record = _history.Record(e, invoker);
            }
        }

        if (eventType.GetCustomAttribute<BroadcastAttribute>() is not null) {
            _signal.Broadcast(e);
        }

        if (eventType.GetCustomAttribute<PamelloEventCategory>() is not { Category: EEventCategory.InfoUpdate }) return record;

        var infoUpdateAttributeData = eventType.GetCustomAttributesData()
            .FirstOrDefault(attr => attr.AttributeType.IsGenericType 
                 && attr.AttributeType.GetGenericTypeDefinition() == typeof(EntityInfoUpdateAttribute<>)
            );
        if (infoUpdateAttributeData is null || eventType.GetCustomAttribute(infoUpdateAttributeData.AttributeType) is not IEntityInfoUpdateAttribute infoUpdateAttribute) return record;
        
        var property = eventType.GetProperties().FirstOrDefault(p => p.Name == infoUpdateAttribute.EntityPropertyName);
        if (property is null || !property.PropertyType.IsAssignableTo(typeof(IPamelloEntity))) return record;

        if (property.GetValue(e) is not IPamelloEntity entity) return record;
        if (entity is IPamelloPlayer player) {
            if (eventType.GetCustomAttribute<BroadcastToPlayerAttribute>() is not null) {
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
