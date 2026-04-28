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

[SafeList<IPamelloPlaylist>("RemovedPlaylists", true)]
public partial class UserFavoritePlaylistsRemoved : UserFavoritePlaylistsUpdated, IRevertiblePamelloEvent
{
    public partial class Pack
    {
        protected override bool DidNotExpireInternal(IPamelloUser scopeUser) {
            return Event.RemovedPlaylists.Any(playlist => !scopeUser.FavoritePlaylists.Contains(playlist));
        }
        protected override void RevertInternal(IPamelloUser scopeUser) {
            if (TryInsert()) return;
            scopeUser.AddFavoritePlaylists(Event.RemovedPlaylists);
            
            return;
            
            bool TryInsert() {
                if (NestedEvent is null) return false;
                
                var removedEvents = NestedEvent.NestedEvents
                    .Select(n => n.Event)
                    .OfType<PlaylistFavoriteByRemoved>()
                    .ToList();

                var first = removedEvents.FirstOrDefault();
                if (first is null) return false;

                if (!Event.FavoritePlaylists.Take(first.RemovedFromPosition).SequenceEqual(
                    scopeUser.FavoriteSongs.Take(first.RemovedFromPosition).Select(s => s.Id)
                )) return false;

                var lastIndex = first.RemovedFromPosition - 1;
                foreach (var removedEvent in removedEvents) {
                    if (removedEvent.RemovedFromPosition != lastIndex + 1) return false;
                    lastIndex = removedEvent.RemovedFromPosition;
                }
                
                scopeUser.AddFavoritePlaylists(removedEvents.Select(e => e.Playlist).OfType<IPamelloPlaylist>(), first.RemovedFromPosition);
                return true;
            }
        }

    }
}
