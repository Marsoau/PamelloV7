using PamelloV7.Wrapper.Events.Base;

namespace PamelloV7.Wrapper.Events.Other;

public class EventSubscription<TEventType> : IEventSubscription
    where TEventType : RemoteEvent
{
    public Type EventType => typeof(TEventType);
    
    public Action<TEventType> Handler { get; }
    
    public EventSubscription(Action<TEventType> handler) {
        Handler = handler;
    }

    public void Invoke(RemoteEvent e) {
        Handler((TEventType)e);
    }
}
