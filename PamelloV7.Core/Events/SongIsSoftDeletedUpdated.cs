using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Events.Enumerators;

namespace PamelloV7.Core.Events;

[Broadcast]
[PamelloEventCategory(EEventCategory.InfoUpdate)]
public class SongIsSoftDeletedUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloSong Song { get; set; }
    public bool IsSoftDeleted { get; set; }
}

