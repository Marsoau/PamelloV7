using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class PlayerNameUpdated : PamelloEvent
    {
        public PlayerNameUpdated() : base(EEventName.PlayerNameUpdated) { }

        public int PlayerId { get; set; }
        public string Name { get; set; }
    }
}

