using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class SongCoverUrlUpdated : PamelloEvent
    {
        public SongCoverUrlUpdated() : base(EEventName.SongCoverUrlUpdated) { }

        public int SongId { get; set; }
        public string CoverUrl { get; set; }
    }
}

