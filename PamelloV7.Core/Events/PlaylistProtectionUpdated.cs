using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlaylistProtectionUpdated : PamelloEvent
    {
        public PlaylistProtectionUpdated() : base(EEventName.PlaylistProtectionUpdated) { }

        public int PlaylistId { get; set; }
        public bool IsProtected { get; set; }
    }
}

