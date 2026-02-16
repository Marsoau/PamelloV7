using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.History.Records;

namespace PamelloV7.Module.Marsoau.Base.Events.Base;

public class EventSubscription<TEventType> : IEventSubscription
    where TEventType : IPamelloEvent
{
    public Type EventType => typeof(TEventType);
    
    public Func<IPamelloUser, TEventType, Task> Handler { get; }
    
    public EventSubscription(Func<TEventType, Task> handler) : this(
        (_, e) => handler(e)
    ) { }
    public EventSubscription(Action<TEventType> handler) : this(
        (_, e) => handler(e)
    ) { }
    
    public EventSubscription(Func<IPamelloUser, TEventType, Task> handler) {
        Handler = handler;
    }
    public EventSubscription(Action<IPamelloUser, TEventType> handler) {
        Handler = (scopeUser, e) => {
            handler.Invoke(scopeUser, e);
            return Task.CompletedTask;
        };
    }

    public Task InvokeAsync(IPamelloUser scopeUser, TEventType e) {
        return Handler(scopeUser, e);
    }

    public Task InvokeAsync(IPamelloUser scopeUser, IPamelloEvent e) {
        return Handler(scopeUser, (TEventType)e);
    }
}
