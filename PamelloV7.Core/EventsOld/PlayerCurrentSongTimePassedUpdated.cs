using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class PlayerCurrentSongTimePassedUpdated : PamelloEvent
    {
        public PlayerCurrentSongTimePassedUpdated() : base(EEventName.PlayerCurrentSongTimePassedUpdated) { }

        public int CurrentSongTimePassed { get; set; }
    }
}

