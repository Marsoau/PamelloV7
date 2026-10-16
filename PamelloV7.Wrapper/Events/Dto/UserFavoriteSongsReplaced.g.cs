//auto generated

using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Attributes;

namespace PamelloV7.Wrapper.Events.Dto;

[EventFullName("PamelloV7.Framework.Events.Actions.UserFavoriteSongsReplaced")]
public class UserFavoriteSongsReplaced : UserFavoriteSongsUpdated
{
    public IEnumerable<System.Int32> PreviousFavoriteSongs { get; set; }
    public IEnumerable<System.Int32> AddedSongs { get; set; }
    public IEnumerable<System.Int32> RemovedSongs { get; set; }

}