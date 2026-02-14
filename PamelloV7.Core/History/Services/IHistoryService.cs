using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.History.Services;

public interface IHistoryService : IPamelloService
{
    public void Record(IPamelloEvent e);
    public void Record(IPamelloEvent nestedEvent, IPamelloEvent parentEvent);
}
