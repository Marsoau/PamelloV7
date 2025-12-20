using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class SongPlayCountUpdated : PamelloEvent
    {
        public SongPlayCountUpdated() : base(EEventName.SongPlayCountUpdated) { }

        public int SongId { get; set; }
        public int PlayCount { get; set; }
    }
}

