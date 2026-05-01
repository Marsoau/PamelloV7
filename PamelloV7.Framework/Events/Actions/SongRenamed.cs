using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;
using PamelloV7.Framework.Events.InfoUpdate;

namespace PamelloV7.Framework.Events.Actions;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Action)]
public partial class SongRenamed : SongNameUpdated, IRevertiblePamelloEvent
{
    public partial class Pack
    {
        protected override bool DidNotExpireInternal(IPamelloUser scopeUser)
            => Event.Song is not null;

        protected override void RevertInternal(IPamelloUser scopeUser) {
            Event.Song?.SetName(Event.OldName, scopeUser);
        }
    }
    
    public required string OldName { get; set; }
}
