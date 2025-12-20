using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class PlayerStateUpdated : PamelloEvent
    {
        public PlayerStateUpdated() : base(EEventName.PlayerStateUpdated) { }

        public EPlayerState State { get; set; }
    }
}

