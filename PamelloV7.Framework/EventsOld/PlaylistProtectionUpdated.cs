using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlaylistProtectionUpdated : PamelloEvent
    {
        public PlaylistProtectionUpdated() : base(EEventName.PlaylistProtectionUpdated) { }

        public int PlaylistId { get; set; }
        public bool IsProtected { get; set; }
    }
}

