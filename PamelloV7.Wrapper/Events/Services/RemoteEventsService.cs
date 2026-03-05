using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using PamelloV7.Wrapper.Entities.Attributes;
using PamelloV7.Wrapper.Entities.Base;
using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Other;
using PamelloV7.Wrapper.Query.Base;

namespace PamelloV7.Wrapper.Events.Services;

public class RemoteEventsService
{
    private readonly PamelloClient _client;
    
    private readonly List<IEventSubscription> _eventSubscriptions;
    private readonly List<UpdateSubscription> _updateSubscriptions;

    private readonly ConcurrentQueue<Task> _updateTasks;
    
    public RemoteEventsService(PamelloClient client) {
        _client = client;
        
        _eventSubscriptions = [];
        _updateSubscriptions = [];
        
        _updateTasks = [];
    }

    private async Task WaitForUpdates() {
        while (true) {
            if (_updateTasks.TryDequeue(out var task)) {
                await task;
            }
            else await Task.Delay(500);
        }
    }

    public UpdateSubscription Watch(Action handler, Func<IRemoteEntity?[]> watchedEntities)
        => Watch(() => {
            handler();
            return Task.CompletedTask;
        }, watchedEntities);
    public UpdateSubscription Watch(Func<Task> handler, Func<IRemoteEntity?[]> watchedEntities) {
        var subscription = new UpdateSubscription(handler, watchedEntities);
        
        _updateSubscriptions.Add(subscription);
        
        return subscription;
    }
    
    public EventSubscription<TEventType> Subscribe<TEventType>(Action<TEventType> handler)
        where TEventType : IRemoteEvent
    {
        var subscription = new EventSubscription<TEventType>(handler);
        
        _eventSubscriptions.Add(subscription);
        
        return subscription;
    }

    protected void UpdateFromEvent(ReceivedEventJsonDto eventDto, IRemoteEvent ev) {
        foreach (var typeInfo in eventDto.Types.Where(typeInfo => !string.IsNullOrEmpty(typeInfo.EntityTypeName))) {
            Debug.WriteLine($"EntityTypeName: {typeInfo.EntityTypeName}");
            Debug.WriteLine($"EntityPropertyName: {typeInfo.EntityPropertyName}");
            Debug.WriteLine($"UpdatePropertyName: {typeInfo.UpdatePropertyName}");
            
            if (ev.GetType().GetProperty(typeInfo.EntityPropertyName)?.GetValue(ev) is not int id) continue;
            
            var parts = typeInfo.UpdatePropertyName.Split('.'); //Queue.Position

            var value = ev.GetType().GetProperty(parts.Last())?.GetValue(ev);
            
            Debug.WriteLine($"Id: {id}");
            Debug.WriteLine($"Value: {value}");
            
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsAssignableTo(typeof(IRemoteEntity)));

            var remoteType = types.FirstOrDefault(type => type.GetCustomAttribute<RemoteEntityInfoAttribute>() is { } attribute && attribute.RemoteInterfaceName == typeInfo.EntityTypeName);
            if (remoteType is null) continue;

            Debug.WriteLine($"RemoteType: {remoteType.Name}");
            
            if (_client.PEQL.GetSingle(remoteType, id) is not { } entity) continue;

            Debug.WriteLine($"Entity: {entity}");
            
            object propertyOwner = entity.Dto;
            var property = propertyOwner.GetType().GetProperty(parts.First());
            
            foreach (var part in parts.Skip(1)) {
                var nextOwner = property?.GetValue(propertyOwner);
                if (nextOwner is null) break;
                
                propertyOwner = nextOwner;
                
                property = propertyOwner.GetType().GetProperty(part);
                if (property is null) break;
            }
            
            if (property is null) continue;

            Debug.WriteLine($"Property: {property.Name}");

            Debug.WriteLine($"Before: {property.GetValue(propertyOwner)}");
            property.SetValue(propertyOwner, value);
            Debug.WriteLine($"After: {property.GetValue(propertyOwner)}");

            foreach (var subscription in _updateSubscriptions.Where(subscription => subscription.WatchedEntities().Contains(entity))) {
                _ = _updateTasks.Append(subscription.InvokeAsync());
            }
        }
    }
    
    internal void Invoke(ReceivedEventJsonDto eventDto) {
        var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(type => eventDto.Types.FirstOrDefault()?.Name is { } name && type.Name == name);
        if (type is null) return;

        if (eventDto.Data.Deserialize(type) is not IRemoteEvent ev) return;
        
        UpdateFromEvent(eventDto, ev);
        
        foreach (var subscription in _eventSubscriptions.Where(subscription =>
            subscription.EventType == typeof(IRemoteEvent) ||
            type.IsAssignableTo(subscription.EventType)
        )) {
            subscription.Invoke(ev);
        }
    }
}
