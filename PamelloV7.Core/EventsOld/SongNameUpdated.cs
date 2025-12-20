using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class SongNameUpdated : PamelloEvent
    {
        public SongNameUpdated() : base(EEventName.SongNameUpdated) { }

        public int SongId { get; set; }
        public string Name { get; set; }
    }
}

