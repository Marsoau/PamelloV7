using PamelloV7.Wrapper.Events.Base;

namespace PamelloV7.Wrapper.Events.Other;

public class EventSubscription<TEventType> : IEventSubscription
    where TEventType : IRemoteEvent
{
    public Type EventType => typeof(TEventType);
    
    public Action<TEventType> Handler { get; }
    
    public EventSubscription(Action<TEventType> handler) {
        Handler = handler;
    }

    public void Invoke(IRemoteEvent e) {
        Handler((TEventType)e);
    }
}
