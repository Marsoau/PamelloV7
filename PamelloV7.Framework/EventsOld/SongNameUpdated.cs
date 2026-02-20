using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class SongNameUpdated : PamelloEvent
    {
        public SongNameUpdated() : base(EEventName.SongNameUpdated) { }

        public int SongId { get; set; }
        public string Name { get; set; }
    }
}

