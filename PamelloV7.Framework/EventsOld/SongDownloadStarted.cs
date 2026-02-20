using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class SongDownloadStarted : PamelloEvent
    {
        public SongDownloadStarted() : base(EEventName.SongDownloadStarted) { }

        public int SongId { get; set; }
    }
}

