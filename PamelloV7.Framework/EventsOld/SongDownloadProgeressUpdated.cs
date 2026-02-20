using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class SongDownloadProgeressUpdated : PamelloEvent
    {
        public SongDownloadProgeressUpdated() : base(EEventName.SongDownloadProgeressUpdated) { }

        public int SongId { get; set; }
        public double Progress { get; set; }
    }
}

