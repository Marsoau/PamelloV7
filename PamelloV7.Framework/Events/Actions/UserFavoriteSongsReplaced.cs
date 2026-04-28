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

[SafeList<IPamelloSong>("PreviousFavoriteSongs", true)]

[SafeList<IPamelloSong>("AddedSongs", true)]
[SafeList<IPamelloSong>("RemovedSongs", true)]
public partial class UserFavoriteSongsReplaced : UserFavoriteSongsUpdated, IRevertiblePamelloEvent
{
    public partial class Pack
    {
        protected override bool DidNotExpireInternal(IPamelloUser scopeUser) {
            return 
                Event.RemovedSongs.Any(song => !scopeUser.FavoriteSongs.Contains(song)) ||
                Event.AddedSongs.Any(song => scopeUser.FavoriteSongs.Contains(song)) ||
                scopeUser.FavoriteSongs.Select(song => song.Id).SequenceEqual(Event.FavoriteSongs);
        }
        protected override void RevertInternal(IPamelloUser scopeUser) {
            if (scopeUser.FavoriteSongs
                .Select(song => song.Id)
                .SequenceEqual(Event.FavoriteSongs)
            ) {
                scopeUser.ReplaceFavoriteSongs(Event.PreviousFavoriteSongs.ToList());
            }
            else {
                scopeUser.AddFavoriteSongs(Event.AddedSongs);
                scopeUser.RemoveFavoriteSongs(Event.RemovedSongs);
            }
        }
    }
}
