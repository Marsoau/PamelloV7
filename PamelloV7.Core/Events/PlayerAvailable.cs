using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlayerAvailable : PamelloEvent
    {
        public PlayerAvailable() : base(EEventName.PlayerAvailable) { }
    }
}

