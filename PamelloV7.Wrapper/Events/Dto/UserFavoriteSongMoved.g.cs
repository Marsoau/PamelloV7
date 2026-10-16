//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.Actions.UserFavoriteSongMoved")]
public class UserFavoriteSongMoved : UserFavoriteSongsUpdated
{
    public System.Int32 FromPosition { get; set; }
    public System.Int32 ToPosition { get; set; }
    public System.Int32? Song { get; set; }

}