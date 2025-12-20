using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class PlaylistCreated : PamelloEvent
    {
        public PlaylistCreated() : base(EEventName.PlaylistCreated) { }

        public int PlaylistId { get; set; }
    }
}

