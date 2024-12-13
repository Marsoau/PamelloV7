using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlayerNextPositionRequestUpdated : PamelloEvent
    {
        public PlayerNextPositionRequestUpdated() : base(EEventName.PlayerNextPositionRequestUpdated) { }
    }
}

