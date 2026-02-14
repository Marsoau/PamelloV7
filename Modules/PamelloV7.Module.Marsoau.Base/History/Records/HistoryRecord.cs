using PamelloV7.Core.Events.Base;
using PamelloV7.Core.History.Records;

namespace PamelloV7.Module.Marsoau.Base.History.Records;

public class HistoryRecord : IHistoryRecord
{
    public IPamelloEvent Event { get; }
    public List<IHistoryRecord> NestedRecords { get; }
    
    public HistoryRecord(IPamelloEvent e) {
        NestedRecords = [];
        
        Event = e;
    }
}
