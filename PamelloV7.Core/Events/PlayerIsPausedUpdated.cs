using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlayerIsPausedUpdated : PamelloEvent
    {
        public PlayerIsPausedUpdated() : base(EEventName.PlayerIsPausedUpdated) { }

        public bool IsPaused { get; set; }
    }
}

