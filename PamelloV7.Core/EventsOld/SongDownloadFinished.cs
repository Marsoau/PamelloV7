using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class SongDownloadFinished : PamelloEvent
    {
        public SongDownloadFinished() : base(EEventName.SongDownloadFinished) { }

        public int SongId { get; set; }
        public EDownloadResult Result { get; set; }
    }
}

