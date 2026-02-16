using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Core.Events.Base;

public interface IUpdateSubscription : IDisposable
{
    public Func<IPamelloEntity[]> WatchedEntities { get; }
    public Task InvokeAsync(IPamelloEvent e);
    
    public bool IsDisposed { get; }
}
