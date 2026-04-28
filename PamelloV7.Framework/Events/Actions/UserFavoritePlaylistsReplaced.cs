using PamelloV7.Core.Entities.Attributes;
using PamelloV7.Framework.Containers;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;
using PamelloV7.Framework.Events.InfoUpdate;

namespace PamelloV7.Framework.Events.Actions;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Action)]

[SafeList<IPamelloPlaylist>("PreviousFavoritePlaylists", true)]

[SafeList<IPamelloPlaylist>("AddedPlaylists", true)]
[SafeList<IPamelloPlaylist>("RemovedPlaylists", true)]
public partial class UserFavoritePlaylistsReplaced : UserFavoritePlaylistsUpdated, IRevertiblePamelloEvent
{
    public partial class Pack
    {
        protected override bool DidNotExpireInternal(IPamelloUser scopeUser) {
            return 
                Event.RemovedPlaylists.Any(playlist => !scopeUser.FavoritePlaylists.Contains(playlist)) ||
                Event.AddedPlaylists.Any(playlist => scopeUser.FavoritePlaylists.Contains(playlist));
        }
        protected override void RevertInternal(IPamelloUser scopeUser) {
            if (scopeUser.FavoritePlaylists
                .Select(playlist => playlist.Id)
                .SequenceEqual(Event.FavoritePlaylists)
            ) {
                scopeUser.ReplaceFavoritePlaylists(Event.PreviousFavoritePlaylists.ToList());
            }
            else {
                scopeUser.AddFavoritePlaylists(Event.AddedPlaylists);
                scopeUser.RemoveFavoritePlaylists(Event.RemovedPlaylists);
            }
        }
    }
}
