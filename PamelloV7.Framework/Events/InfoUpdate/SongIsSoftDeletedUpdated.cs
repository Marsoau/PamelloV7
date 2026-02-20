using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.InfoUpdate;

[Broadcast]
[PamelloEventCategory(EEventCategory.InfoUpdate)]
public partial class SongIsSoftDeletedUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloSong Song { get; set; }
    public bool IsSoftDeleted { get; set; }
}

