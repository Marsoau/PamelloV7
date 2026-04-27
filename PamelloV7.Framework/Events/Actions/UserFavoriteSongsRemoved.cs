using PamelloV7.Core.Entities.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;
using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Framework.Logging;

namespace PamelloV7.Framework.Events.Actions;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Action)]

[SafeList<IPamelloSong>("RemovedSongs", true)]
public partial class UserFavoriteSongsRemoved : UserFavoriteSongsUpdated, IRevertiblePamelloEvent
{
    public partial class Pack
    {
        protected override bool DidNotExpireInternal(IPamelloUser scopeUser) {
            return Event.RemovedSongs.Any(song => !scopeUser.FavoriteSongs.Contains(song));
        }
        protected override void RevertInternal(IPamelloUser scopeUser) {
            scopeUser.AddFavoriteSongs(Event.RemovedSongs);
        }
    }
}
