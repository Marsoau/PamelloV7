//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.Actions.SongAddedToQueue")]
public class SongAddedToQueue : PlayerQueueEntriesUpdated
{
    public System.Int32 InsertPosition { get; set; }
    public IEnumerable<System.Int32> AddedSongs { get; set; }

}