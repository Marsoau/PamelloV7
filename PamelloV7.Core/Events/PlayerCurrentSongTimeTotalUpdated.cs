using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlayerCurrentSongTimeTotalUpdated : PamelloEvent
    {
        public PlayerCurrentSongTimeTotalUpdated() : base(EEventName.PlayerCurrentSongTimeTotalUpdated) { }

        public int CurrentSongTimeTotal { get; set; }
    }
}

