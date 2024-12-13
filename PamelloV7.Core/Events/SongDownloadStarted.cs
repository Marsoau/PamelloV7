using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class SongDownloadStarted : PamelloEvent
    {
        public SongDownloadStarted() : base(EEventName.SongDownloadStarted) { }

        public int SongId { get; set; }
    }
}

