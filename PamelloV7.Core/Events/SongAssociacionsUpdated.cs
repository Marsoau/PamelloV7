using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class SongAssociacionsUpdated : PamelloEvent
    {
        public SongAssociacionsUpdated() : base(EEventName.SongAssociacionsUpdated) { }

        public int SongId { get; set; }
        public IEnumerable<string> Associacions { get; set; }
    }
}

