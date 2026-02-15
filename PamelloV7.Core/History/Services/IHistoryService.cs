using PamelloV7.Core.Events.Base;
using PamelloV7.Core.History.Records;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.History.Services;

public interface IHistoryService : IPamelloService
{
    public IHistoryRecord GetRequired(int id);
    public IHistoryRecord Get(int id);
    
    public void Record(IPamelloEvent e);
    public void Record(IPamelloEvent nestedEvent, IPamelloEvent parentEvent);
}
