using System.Reflection;
using System.Text.Json;
using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Other;

namespace PamelloV7.Wrapper.Events.Services;

public class RemoteEventsService
{
    private readonly List<IEventSubscription> _subscriptions;
    
    public RemoteEventsService() {
        _subscriptions = [];
    }
    
    public EventSubscription<TEventType> Subscribe<TEventType>(Action<TEventType> handler)
        where TEventType : IRemoteEvent
    {
        var subscription = new EventSubscription<TEventType>(handler);
        
        _subscriptions.Add(subscription);
        
        return subscription;
    }
    
    internal void Invoke(ReceivedEventJsonDto eventDto) {
        var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(type => eventDto.Types.FirstOrDefault()?.Name is { } name && type.Name == name);
        if (type is null) return;

        if (eventDto.Data.Deserialize(type) is not IRemoteEvent ev) return;
        
        foreach (var subscription in _subscriptions.Where(subscription =>
            subscription.EventType == typeof(IRemoteEvent) ||
            type.IsAssignableTo(subscription.EventType)
        )) {
            subscription.Invoke(ev);
        }
    }
}
