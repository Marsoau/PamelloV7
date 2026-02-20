using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.History.Records;

namespace PamelloV7.Module.Marsoau.Base.Events.Base;

public class EventSubscription<TEventType> : IEventSubscription
    where TEventType : IPamelloEvent
{
    public Type EventType => typeof(TEventType);
    
    public Action<TEventType> Handler { get; }
    
    public EventSubscription(Action<TEventType> handler) {
        Handler = handler;
    }

    public void Invoke(IPamelloEvent e) {
        Handler((TEventType)e);
    }
}
