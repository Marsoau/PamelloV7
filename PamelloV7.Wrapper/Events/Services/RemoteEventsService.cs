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
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).ToList();
        var type = types.FirstOrDefault(type => type.Name == eventDto.Type.Name);
        
        if (type is null) return;

        if (eventDto.Data.Deserialize(type) is not IRemoteEvent ev) return;
        
        foreach (var subscription in _subscriptions.Where(subscription =>
            subscription.EventType == typeof(IRemoteEvent) ||
            subscription.EventType.IsAssignableTo(subscription.EventType)
        )) {
            subscription.Invoke(ev);
        }
    }
}
