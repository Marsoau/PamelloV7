using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class SongUpdated : PamelloEvent
    {
        public SongUpdated() : base(EEventName.SongUpdated) { }
    }
}

