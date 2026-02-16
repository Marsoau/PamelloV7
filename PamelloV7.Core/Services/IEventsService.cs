using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.History.Records;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Services;

public interface IEventsService : IPamelloService
{
    public IEventSubscription Subscribe<TEventType>(Func<TEventType, Task> handler)
        where TEventType : IPamelloEvent;
    public IEventSubscription Subscribe<TEventType>(Func<IPamelloUser?, TEventType, Task> handler)
        where TEventType : IPamelloEvent;
    public IEventSubscription Subscribe<TEventType>(Action<TEventType> handler)
        where TEventType : IPamelloEvent;
    public IEventSubscription Subscribe<TEventType>(Action<IPamelloUser?, TEventType> handler)
        where TEventType : IPamelloEvent;

    public IUpdateSubscription Watch(Func<IPamelloEvent, Task> handler, Func<IPamelloEntity?[]> watchedEntities);
    
    public Task<IHistoryRecord?> InvokeAsync(Type type, IPamelloUser? invoker, IPamelloEvent e);
    public Task<IHistoryRecord?> InvokeAsync<TPamelloEvent>(IPamelloUser? invoker, TPamelloEvent e)
        where TPamelloEvent : IPamelloEvent;
}
