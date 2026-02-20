using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlayerStateUpdated : PamelloEvent
    {
        public PlayerStateUpdated() : base(EEventName.PlayerStateUpdated) { }

        public EPlayerState State { get; set; }
    }
}

