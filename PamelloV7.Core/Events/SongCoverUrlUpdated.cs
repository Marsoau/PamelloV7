using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class SongCoverUrlUpdated : PamelloEvent
    {
        public SongCoverUrlUpdated() : base(EEventName.SongCoverUrlUpdated) { }

        public int SongId { get; set; }
        public string CoverUrl { get; set; }
    }
}

