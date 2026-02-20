using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Events.Enumerators;

namespace PamelloV7.Core.Events.InfoUpdate;

[Broadcast]
[PamelloEventCategory(EEventCategory.InfoUpdate)]
public class EpisodeNameUpdated : IPamelloEvent
{
    [InfoUpdateProperty]
    public IPamelloEpisode Episode { get; set; }
    
    public string NewName { get; set; }
}

