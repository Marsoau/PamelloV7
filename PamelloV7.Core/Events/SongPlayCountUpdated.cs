using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class SongPlayCountUpdated : PamelloEvent
    {
        public SongPlayCountUpdated() : base(EEventName.SongPlaycountUpdated) { }

        public int SongId { get; set; }
        public int PlayCount { get; set; }
    }
}

