using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.History.Records;

namespace PamelloV7.Module.Marsoau.Base.Events.Base;

public class EventSubscription<TEventType> : IEventSubscription
    where TEventType : IPamelloEvent
{
    public Type EventType => typeof(TEventType);
    
    public Action<IPamelloUser?, TEventType> Handler { get; }
    
    public EventSubscription(Action<TEventType> handler) : this(
        (_, e) => handler(e)
    ) { }
    
    public EventSubscription(Action<IPamelloUser?, TEventType> handler) {
        Handler = handler;
    }

    public void Invoke(IPamelloUser? scopeUser, IPamelloEvent e) {
        Handler(scopeUser, (TEventType)e);
    }
}
