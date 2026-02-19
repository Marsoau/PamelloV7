using PamelloV7.Core.Attributes;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;

namespace PamelloV7.Core.History.Records;

[ValueEntity("history")]
public interface IHistoryRecord : IPamelloEntity
{
    public int Id { get; set; }
    
    public IPamelloUser? Performer { get; set; }
    public NestedPamelloEvent Nested { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public void Revert(IPamelloUser scopeUser);
}
