using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlayerCurrentSongTimePassedUpdated : PamelloEvent
    {
        public PlayerCurrentSongTimePassedUpdated() : base(EEventName.PlayerCurrentSongTimePassedUpdated) { }

        public int CurrentSongTimePassed { get; set; }
    }
}

