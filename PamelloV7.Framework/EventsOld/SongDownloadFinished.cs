using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class SongDownloadFinished : PamelloEvent
    {
        public SongDownloadFinished() : base(EEventName.SongDownloadFinished) { }

        public int SongId { get; set; }
        public EDownloadResult Result { get; set; }
    }
}

