using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class UserAvatarUpdated : PamelloEvent
    {
        public UserAvatarUpdated() : base(EEventName.UserAvatarUpdated) { }
    }
}

