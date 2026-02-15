using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.History.Records;

namespace PamelloV7.Module.Marsoau.Base.History.Records;

public class HistoryRecord : IHistoryRecord
{
    public int Id { get; set; }
    
    public IPamelloEvent Event { get; set; }
    public List<IHistoryRecord> NestedRecords { get; set; }
    
    public bool IsRevertible => Event is RevertiblePamelloEvent;
    
    public HistoryRecord() { }
    public HistoryRecord(IPamelloEvent e) {
        NestedRecords = [];
        
        Event = e;
    }

    public void Revert()
        => (Event as RevertiblePamelloEvent ?? throw new PamelloException("Event is not revertible")).RevertPack.Revert();
}
