using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Services;

public interface IEventsService : IPamelloService
{
    public IEventSubscription Subscribe<TEventType>(Func<TEventType, Task> handler)
        where TEventType : IPamelloEvent;

    public IUpdateSubscription Watch(Func<IPamelloEvent, Task> handler, Func<IPamelloEntity[]> watchedEntities);
    
    public IPamelloEvent Invoke(Type type, IPamelloEvent e);
    public TPamelloEvent Invoke<TPamelloEvent>(TPamelloEvent e)
        where TPamelloEvent : IPamelloEvent;
}
