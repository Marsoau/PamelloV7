using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Services;

public interface ISSEBroadcastService : IPamelloService
{
    public void Broadcast(IPamelloEvent e);
    public void BroadcastToPlayer(IPamelloEvent e, IPamelloPlayer? player);
}
