using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlaylistDeleted : PamelloEvent
    {
        public PlaylistDeleted() : base(EEventName.PlaylistDeleted) { }
    }
}

