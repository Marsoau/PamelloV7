using PamelloV7.Core.Entities.Attributes;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Containers;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;
using PamelloV7.Framework.Events.InfoUpdate;
using PamelloV7.Framework.Events.RestorePacks.Base;

namespace PamelloV7.Framework.Events.Actions;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Action)]

[SafeEntities<IPamelloSong>("AddedSongs")]
public partial class SongAddedToQueue : PlayerQueueEntriesUpdated, IRevertiblePamelloEvent
{
    public partial class Pack
    {
        protected override void RevertInternal(IPamelloUser scopeUser) {
            Event.Player!.RequiredQueue.RemoveSongsRange((Event.InsertPosition + 1).ToString(), (Event.InsertPosition + Event.AddedSongs.Count()).ToString(), scopeUser);
        }

        protected override bool DidNotExpireInternal(IPamelloUser scopeUser) {
            if (Event.Player != scopeUser.SelectedPlayer || Event.Player is null) return false;
            return false; //scopeUser.SelectedPlayer?.Queue?.Entries.SequenceEqual(Event.Entries) ?? false;
        }
    }
    
    public required int InsertPosition { get; set; }
}

