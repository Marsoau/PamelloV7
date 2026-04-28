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
            if (TryInsert()) return;
            scopeUser.AddFavoriteSongs(Event.RemovedSongs);
            
            return;

            bool TryInsert() {
                if (NestedEvent is null) return false;
                
                var removedEvents = NestedEvent.NestedEvents
                    .Select(n => n.Event)
                    .OfType<SongFavoriteByRemoved>()
                    .ToList();

                var first = removedEvents.FirstOrDefault();
                if (first is null) return false;

                if (!Event.FavoriteSongs.Take(first.RemovedFromPosition).SequenceEqual(
                    scopeUser.FavoriteSongs.Take(first.RemovedFromPosition).Select(s => s.Id)
                )) return false;

                var lastIndex = first.RemovedFromPosition - 1;
                foreach (var removedEvent in removedEvents) {
                    if (removedEvent.RemovedFromPosition != lastIndex + 1) return false;
                    lastIndex = removedEvent.RemovedFromPosition;
                }
                
                scopeUser.AddFavoriteSongs(removedEvents.Select(e => e.Song).OfType<IPamelloSong>(), first.RemovedFromPosition);
                return true;
            }
        }
    }
}
