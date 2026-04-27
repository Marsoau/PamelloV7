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

[SafeList<IPamelloSong>("AddedSongs", true)]
public partial class UserFavoriteSongsAdded : UserFavoriteSongsUpdated, IRevertiblePamelloEvent
{
    public partial class Pack
    {
        protected override bool DidNotExpireInternal(IPamelloUser scopeUser) {
            return scopeUser.FavoriteSongs.Select(song => song.Id).SequenceEqual(Event.FavoriteSongs);
        }
        protected override void RevertInternal(IPamelloUser scopeUser) {
            var modifiedSongs = scopeUser.FavoriteSongs.ToList();
            Output.Write($"Reverting: {modifiedSongs.Count}, {Event.InsertPosition}, {Event.AddedSongs.Count()}");
            modifiedSongs.RemoveRange(Event.InsertPosition, Event.AddedSongs.Count());
            
            scopeUser.ReplaceFavoriteSongs(modifiedSongs);
        }
    }
    
    public required int InsertPosition { get; set; }
}
