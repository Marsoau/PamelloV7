using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.InfoUpdate;

[Broadcast]
[PamelloEventCategory(EEventCategory.InfoUpdate)]
public partial class UserSelectedAuthorizationIndexUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloUser User { get; set; }
    public int SelectedAuthorizationIndex { get; set; }
}
