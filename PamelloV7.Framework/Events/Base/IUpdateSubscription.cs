using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.Events.Base;

public interface IUpdateSubscription : IDisposable
{
    public Func<IPamelloEntity?[]> WatchedEntities { get; }
    public Task InvokeAsync(IPamelloEvent e);
    
    public bool IsDisposed { get; }
}
