using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class PlayerNextPositionRequestUpdated : PamelloEvent
    {
        public PlayerNextPositionRequestUpdated() : base(EEventName.PlayerNextPositionRequestUpdated) { }

        public int? NextPositionRequest { get; set; }
    }
}

