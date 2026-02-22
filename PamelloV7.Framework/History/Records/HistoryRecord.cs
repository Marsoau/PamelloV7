using PamelloV7.Core.Dto;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.DTO;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Exceptions;

namespace PamelloV7.Framework.History.Records;

public class HistoryRecord : IHistoryRecord
{
    public int Id { get; set; }
    public string Name => Nested.Event.GetType().Name;

    public IPamelloUser? Performer { get; set; }
    public NestedPamelloEvent Nested { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public HistoryRecord() { }
    public HistoryRecord(NestedPamelloEvent nested, IPamelloUser? performer) {
        Nested = nested;
        Performer = performer;
        
        CreatedAt = DateTime.UtcNow;
    }
    
    public void Revert(IPamelloUser scopeUser) {
        if (!Nested.IsRevertible()) throw new PamelloException("Event is not revertible");
        
        Nested.Revert(scopeUser);
    }
    
    public PamelloEntityDto GetDto() {
        return new HistoryRecordDTO() {
            Id = Id,
            Name = Name,
            PerformerId = Performer?.Id,
            Nested = Nested.GetDto(),
            CreatedAt = CreatedAt
        };
    }
}
