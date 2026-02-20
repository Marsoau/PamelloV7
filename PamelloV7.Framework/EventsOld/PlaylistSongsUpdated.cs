using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlaylistSongsUpdated : PamelloEvent
    {
        public PlaylistSongsUpdated() : base(EEventName.PlaylistSongsUpdated) { }

        public int PlaylistId { get; set; }
        public IEnumerable<int> SongsIds { get; set; }
    }
}

