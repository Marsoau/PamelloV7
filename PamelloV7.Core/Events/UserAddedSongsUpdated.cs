using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class UserAddedSongsUpdated : PamelloEvent
    {
        public UserAddedSongsUpdated() : base(EEventName.UserAddedSongsUpdated) { }
    }
}

