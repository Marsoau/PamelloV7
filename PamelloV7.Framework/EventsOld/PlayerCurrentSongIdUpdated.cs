using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlayerCurrentSongIdUpdated : PamelloEvent
    {
        public PlayerCurrentSongIdUpdated() : base(EEventName.PlayerCurrentSongIdUpdated) { }

        public int? CurrentSongId { get; set; }
    }
}

