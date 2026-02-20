using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlayerProtectionUpdated : PamelloEvent
    {
        public PlayerProtectionUpdated() : base(EEventName.PlayerProtectionUpdated) { }

        public bool IsProtected { get; set; }
    }
}
