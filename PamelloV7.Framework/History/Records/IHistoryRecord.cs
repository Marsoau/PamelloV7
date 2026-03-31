using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Dto;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.History.Records;

[PamelloEntity("history", typeof(HistoryRecordDto))]
public interface IHistoryRecord : IPamelloEntity
{
    public IPamelloUser? Performer { get; set; }
    public NestedPamelloEvent Nested { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public void Revert(IPamelloUser scopeUser);
}
