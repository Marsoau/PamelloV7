using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlayerIsPausedUpdated : PamelloEvent
    {
        public PlayerIsPausedUpdated() : base(EEventName.PlayerIsPausedUpdated) { }

        public bool IsPaused { get; set; }
    }
}

