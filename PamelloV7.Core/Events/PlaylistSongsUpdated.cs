using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlaylistSongsUpdated : PamelloEvent
    {
        public PlaylistSongsUpdated() : base(EEventName.PlaylistSongsUpdated) { }

        public int PlaylistId { get; set; }
        public IEnumerable<int> SongsIds { get; set; }
    }
}

