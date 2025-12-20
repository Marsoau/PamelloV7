using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class SongDownloadStarted : PamelloEvent
    {
        public SongDownloadStarted() : base(EEventName.SongDownloadStarted) { }

        public int SongId { get; set; }
    }
}

