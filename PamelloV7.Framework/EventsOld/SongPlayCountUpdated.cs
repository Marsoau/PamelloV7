using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class SongPlayCountUpdated : PamelloEvent
    {
        public SongPlayCountUpdated() : base(EEventName.SongPlayCountUpdated) { }

        public int SongId { get; set; }
        public int PlayCount { get; set; }
    }
}

