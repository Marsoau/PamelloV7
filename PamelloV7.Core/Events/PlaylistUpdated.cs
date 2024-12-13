using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlaylistUpdated : PamelloEvent
    {
        public PlaylistUpdated() : base(EEventName.PlaylistUpdated) { }
    }
}

