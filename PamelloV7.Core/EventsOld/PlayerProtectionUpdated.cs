using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class PlayerProtectionUpdated : PamelloEvent
    {
        public PlayerProtectionUpdated() : base(EEventName.PlayerProtectionUpdated) { }

        public bool IsProtected { get; set; }
    }
}
