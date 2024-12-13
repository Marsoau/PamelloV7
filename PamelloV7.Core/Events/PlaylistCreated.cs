using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlaylistCreated : PamelloEvent
    {
        public PlaylistCreated() : base(EEventName.PlaylistCreated) { }
    }
}

