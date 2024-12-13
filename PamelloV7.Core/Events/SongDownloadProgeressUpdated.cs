using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class SongDownloadProgeressUpdated : PamelloEvent
    {
        public SongDownloadProgeressUpdated() : base(EEventName.SongDownloadProgeressUpdated) { }

        public int SongId { get; set; }
        public double Progress { get; set; }
    }
}

