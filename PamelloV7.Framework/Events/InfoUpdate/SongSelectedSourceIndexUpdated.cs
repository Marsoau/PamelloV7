using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.InfoUpdate;

[Broadcast]
[PamelloEventCategory(EEventCategory.InfoUpdate)]
public partial class SongSelectedSourceIndexUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloSong Song { get; set; }
    public int SelectedSourceIndex { get; set; }
}
