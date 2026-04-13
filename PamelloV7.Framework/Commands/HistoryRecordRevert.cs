using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.History.Records;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class HistoryRecordRevert
{
    public void Execute(IHistoryRecord record) {
        record.Revert(ScopeUser);
    }
}

