using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.History.Records;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.History.Services;

public interface IHistoryService : IPamelloService
{
    public HistoryRecord GetRequired(int id);
    public HistoryRecord? Get(int id);
    
    public HistoryRecord Record(IPamelloEvent e, IPamelloUser? scopeUser);
    public void Record(IPamelloEvent nestedEvent, IPamelloEvent parentEvent);

    public void FullReset();
}
