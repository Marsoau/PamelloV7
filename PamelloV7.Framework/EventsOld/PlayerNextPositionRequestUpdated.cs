using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlayerNextPositionRequestUpdated : PamelloEvent
    {
        public PlayerNextPositionRequestUpdated() : base(EEventName.PlayerNextPositionRequestUpdated) { }

        public int? NextPositionRequest { get; set; }
    }
}

