using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class PlaylistNameUpdated : PamelloEvent
    {
        public PlaylistNameUpdated() : base(EEventName.PlaylistNameUpdated) { }

        public int PlaylistId { get; set; }
        public string Name { get; set; }
    }
}

