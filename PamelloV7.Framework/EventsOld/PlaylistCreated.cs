using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlaylistCreated : PamelloEvent
    {
        public PlaylistCreated() : base(EEventName.PlaylistCreated) { }

        public int PlaylistId { get; set; }
    }
}

