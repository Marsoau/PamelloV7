using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlayerCurrentSongTimeTotalUpdated : PamelloEvent
    {
        public PlayerCurrentSongTimeTotalUpdated() : base(EEventName.PlayerCurrentSongTimeTotalUpdated) { }

        public int CurrentSongTimeTotal { get; set; }
    }
}

