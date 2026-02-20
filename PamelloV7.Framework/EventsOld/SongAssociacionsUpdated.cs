using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class SongAssociacionsUpdated : PamelloEvent
    {
        public SongAssociacionsUpdated() : base(EEventName.SongAssociacionsUpdated) { }

        public int SongId { get; set; }
        public IEnumerable<string> Associacions { get; set; }
    }
}

