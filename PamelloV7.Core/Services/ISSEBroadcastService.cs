using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Services;

public interface ISSEBroadcastService : IPamelloService
{
    public void Broadcast(IPamelloEvent e);
}
