using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlayerUpdated : PamelloEvent
    {
        public PlayerUpdated() : base(EEventName.PlayerUpdated) { }
    }
}

