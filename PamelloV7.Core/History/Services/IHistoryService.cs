using PamelloV7.Core.Attributes;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.History.Records;
using PamelloV7.Core.Services.Base;
using PamelloV7.Core.Services.PEQL;

namespace PamelloV7.Core.History.Services;

[EntityProvider("history")]
public interface IHistoryService : IPamelloService, IEntityProvider
{
    [IdPoint]
    public IHistoryRecord? Get(IPamelloUser scopeUser, int id);
    
    [ValuePoint("all")]
    public IEnumerable<IHistoryRecord> GetAll(IPamelloUser scopeUser);
    
    [ValuePoint("last")]
    public IEnumerable<IHistoryRecord> GetLast(IPamelloUser scopeUser, int count = 1);
    
    public IHistoryRecord? GetRequired(int id);
    public IHistoryRecord? Get(int id);
    
    public IHistoryRecord Record(IPamelloEvent e, IPamelloUser? scopeUser);
    public void Record(IPamelloEvent nestedEvent, IPamelloEvent parentEvent);

    public void FullReset();
    public void WriteAll();
}
