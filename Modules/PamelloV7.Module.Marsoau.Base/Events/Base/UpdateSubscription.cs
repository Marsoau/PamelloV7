using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Events.Base;

namespace PamelloV7.Module.Marsoau.Base.Events.Base;

public class UpdateSubscription : IUpdateSubscription
{
    public IPamelloEntity[] WatchedEntities { get; }
    public Func<IPamelloEvent, Task> Handler { get; }
    
    public bool IsDisposed { get; private set; }
    
    public UpdateSubscription(Func<IPamelloEvent, Task> handler, params IPamelloEntity[] watchedEntities) {
        WatchedEntities = watchedEntities;
        Handler = handler;
    }

    public Task Invoke(IPamelloEvent e) {
        return Handler(e);
    }

    public void Dispose() {
        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}
