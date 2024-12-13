using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class PlayerCurrentEpisodePositionUpdated : PamelloEvent
    {
        public PlayerCurrentEpisodePositionUpdated() : base(EEventName.PlayerCurrentEpisodePositionUpdated) { }
    }
}

