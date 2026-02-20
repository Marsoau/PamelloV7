using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.History.Records;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Framework.Services.PEQL;

namespace PamelloV7.Framework.History.Services;

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
