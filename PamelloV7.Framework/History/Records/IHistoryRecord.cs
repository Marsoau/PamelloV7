using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.History.Records;

[ValueEntity("history")]
public interface IHistoryRecord : IPamelloEntity
{
    public int Id { get; set; }
    
    public IPamelloUser? Performer { get; set; }
    public NestedPamelloEvent Nested { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public void Revert(IPamelloUser scopeUser);
}
