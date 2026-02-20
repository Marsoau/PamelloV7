using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Miscellaneous;
using PamelloV7.Framework.Events.RestorePacks.Base;

namespace PamelloV7.Module.Marsoau.Base.Events.RevertPacks;

public class SongAddedToQueueRevertPack : RevertPack<SongAddedToQueue>
{
    protected override void RevertInternal(IPamelloUser scopeUser) {
        Event.Player.RequiredQueue.RemoveSongsRange((Event.QueuePosition + 1).ToString(), (Event.QueuePosition + Event.AddedSongs.Count() + 1).ToString(), scopeUser);
    }
}
