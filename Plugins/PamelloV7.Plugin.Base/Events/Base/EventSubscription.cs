using PamelloV7.Core.Events.Base;

namespace PamelloV7.Plugin.Base.Events.Base;

public class EventSubscription<TEventType> : IEventSubscription
    where TEventType : IPamelloEvent
{
    public Type Type => typeof(TEventType);
    public Func<TEventType, Task> Handler { get; }
    
    public EventSubscription(Func<TEventType, Task> handler) {
        Handler = handler;
    }

    public Task Invoke(TEventType e) {
        return Handler(e);
    }

    public Task Invoke(IPamelloEvent e) {
        return Handler((TEventType)e);
    }
}
