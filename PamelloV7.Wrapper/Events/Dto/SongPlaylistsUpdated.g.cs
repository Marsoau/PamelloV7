//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.InfoUpdate.SongPlaylistsUpdated")]
public class SongPlaylistsUpdated : RemoteEvent
{
    public IEnumerable<System.Int32> Playlists { get; set; }
    public System.Int32 Invoker { get; set; }
    public System.Int32? Song { get; set; }

}