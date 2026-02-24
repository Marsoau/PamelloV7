using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Framework.Events.InfoUpdate;

[Broadcast]
[PamelloEventCategory(EEventCategory.InfoUpdate)]

[SafeEntity<IPamelloSong>("Song", typeof(InfoUpdatePropertyAttribute))]
public partial class SongAssociationsUpdated : IPamelloEvent
{
    public IEnumerable<string> Associations { get; set; }
}
