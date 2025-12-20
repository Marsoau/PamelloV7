using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class PlaylistDeleted : PamelloEvent
    {
        public PlaylistDeleted() : base(EEventName.PlaylistDeleted) { }
    }
}

