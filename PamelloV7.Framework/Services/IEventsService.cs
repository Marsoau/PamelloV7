using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.History.Records;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Services;

public delegate IPamelloEntity?[] GetEntities();

public interface IEventsService : IPamelloService
{
    public IEventSubscription Subscribe<TEventType>(Action<TEventType> handler)
        where TEventType : IPamelloEvent;

    public IUpdateSubscription Watch(Func<IPamelloEvent, Task> handler, GetEntities watchedEntities);
    
    public IHistoryRecord? Invoke(IPamelloUser? invoker, IPamelloEvent e);
    public IHistoryRecord? Invoke(IPamelloUser? invoker, IPamelloEvent e, Action? action);
}
