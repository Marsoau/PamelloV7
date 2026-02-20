using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlaylistNameUpdated : PamelloEvent
    {
        public PlaylistNameUpdated() : base(EEventName.PlaylistNameUpdated) { }

        public int PlaylistId { get; set; }
        public string Name { get; set; }
    }
}

