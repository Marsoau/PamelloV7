using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.History.Records;

public interface IHistoryRecord
{
    public IPamelloEvent Event { get; }
    public List<IHistoryRecord> NestedRecords { get; }
}
