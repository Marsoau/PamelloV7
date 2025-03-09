using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlayerNameUpdated : PamelloEvent
    {
        public PlayerNameUpdated() : base(EEventName.PlayerNameUpdated) { }

        public int PlayerId { get; set; }
        public string Name { get; set; }
    }
}

