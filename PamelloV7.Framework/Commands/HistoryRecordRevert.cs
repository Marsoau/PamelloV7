using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.History.Records;

namespace PamelloV7.Framework.Commands;

public class HistoryRecordRevert : PamelloCommand
{
    public void Execute(IHistoryRecord record) {
        record.Revert(ScopeUser);
    }
}

