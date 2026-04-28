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

[SafeList<IPamelloPlaylist>("AddedPlaylists", true)]
public partial class UserFavoritePlaylistsAdded : UserFavoritePlaylistsUpdated, IRevertiblePamelloEvent
{
    public partial class Pack
    {
        protected override bool DidNotExpireInternal(IPamelloUser scopeUser) {
            return Event.AddedPlaylists.Any(playlist => scopeUser.FavoritePlaylists.Contains(playlist));
        }
        protected override void RevertInternal(IPamelloUser scopeUser) {
            scopeUser.RemoveFavoritePlaylists(Event.AddedPlaylists);
        }
    }
}
