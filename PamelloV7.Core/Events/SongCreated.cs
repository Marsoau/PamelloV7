using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class SongCreated : PamelloEvent
    {
        public SongCreated() : base(EEventName.SongCreated) { }
    }
}

