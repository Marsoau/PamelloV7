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
public partial class SongAddedToQueue : PlayerQueueEntriesUpdated, IRevertiblePamelloEvent
{
    public class Pack : RevertPack<SongAddedToQueue>
    {
        protected override void RevertInternal(IPamelloUser scopeUser) {
            Event.Player.RequiredQueue.RemoveSongsRange((Event.QueuePosition + 1).ToString(), (Event.QueuePosition + Event.AddedSongs.Count()).ToString(), scopeUser);
        }

        protected override bool DidNotExpireInternal(IPamelloUser scopeUser) {
            if (Event.Player != scopeUser.SelectedPlayer) return false;
            return scopeUser.SelectedPlayer?.Queue?.Entries.SequenceEqual(Event.Entries) ?? false;
        }
    }
    
    public SafeStoredEntities<IPamelloSong> AddedSongs { get; set; }
    public int QueuePosition { get; set; }
}

