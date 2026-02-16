using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.History.Records;

namespace PamelloV7.Module.Marsoau.Base.History.Records;

public class HistoryRecord : IHistoryRecord
{
    public int Id { get; set; }

    public IPamelloUser? Performer { get; set; }
    public IPamelloEvent Event { get; set; }
    public List<IHistoryRecord> NestedRecords { get; set; }

    public bool IsRevertible => Event is RevertiblePamelloEvent { RevertPack.IsActivated: true };
    
    public HistoryRecord() { }
    public HistoryRecord(IPamelloEvent e, IPamelloUser? performer) {
        NestedRecords = [];
        
        Event = e;
        Performer = performer;
    }

    public void Revert(IPamelloUser scopeUser) {
        if (!IsRevertible) throw new PamelloException("Event is not revertible");
        
        ((RevertiblePamelloEvent)Event).RevertPack.Revert(scopeUser);

        foreach (var nestedRecord in NestedRecords.Where(nestedRecord => nestedRecord.IsRevertible)) {
            nestedRecord.Revert(scopeUser);
        }
    }
}
