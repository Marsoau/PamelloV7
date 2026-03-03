//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.InfoUpdate.UserFavoriteSongsUpdated")]
public class UserFavoriteSongsUpdated : IRemoteEvent
{
    public IEnumerable<System.Int32> FavoriteSongsIds { get; set; }
    public System.Int32? Invoker { get; set; }
    public System.Int32? User { get; set; }

}