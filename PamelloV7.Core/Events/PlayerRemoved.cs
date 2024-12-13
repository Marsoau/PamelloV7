using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlayerRemoved : PamelloEvent
    {
        public PlayerRemoved() : base(EEventName.PlayerRemoved) { }
    }
}

