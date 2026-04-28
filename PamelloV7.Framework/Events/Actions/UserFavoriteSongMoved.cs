using PamelloV7.Core.Entities.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;
using PamelloV7.Framework.Events.InfoUpdate;

namespace PamelloV7.Framework.Events.Actions;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Action)]

[Safe<IPamelloSong>("Song", true)]
public partial class UserFavoriteSongMoved : UserFavoriteSongsUpdated, IRevertiblePamelloEvent
{
    public partial class Pack
    {
        protected override bool DidNotExpireInternal(IPamelloUser scopeUser) {
            return scopeUser != Event.User &&
                Event.FavoriteSongs.SequenceEqual(scopeUser.FavoriteSongs.Select(song => song.Id));
        }

        protected override void RevertInternal(IPamelloUser scopeUser) {
            scopeUser.MoveFavoriteSong(Event.ToPosition, Event.FromPosition);
        }
    }
    
    public required int FromPosition { get; set; }
    public required int ToPosition { get; set; }
}
