using PamelloV7.Framework.Entities.Base;
using PamelloV7.Framework.Services;

namespace PamelloV7.Framework.Events.Base;

public interface IUpdateSubscription : IDisposable
{
    public GetEntities WatchedEntities { get; }
    public Task InvokeAsync(IPamelloEvent e);
    
    public bool IsDisposed { get; }
}
