using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Services;

public interface ISignalBroadcastService : IPamelloService
{
    public void Broadcast(IPamelloEvent e);
    public void BroadcastToPlayer(IPamelloEvent e, IPamelloPlayer? player);
}
