using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Core.Events.Base;

public interface IUpdateSubscription : IDisposable
{
    public IPamelloEntity[] WatchedEntities { get; }
    public Task Invoke(IPamelloEvent e);
    
    public bool IsDisposed { get; }
}
