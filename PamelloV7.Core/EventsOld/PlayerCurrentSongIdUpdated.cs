using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class PlayerCurrentSongIdUpdated : PamelloEvent
    {
        public PlayerCurrentSongIdUpdated() : base(EEventName.PlayerCurrentSongIdUpdated) { }

        public int? CurrentSongId { get; set; }
    }
}

