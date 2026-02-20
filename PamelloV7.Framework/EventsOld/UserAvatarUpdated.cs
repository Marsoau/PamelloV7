using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class UserAvatarUpdated : PamelloEvent
    {
        public UserAvatarUpdated() : base(EEventName.UserAvatarUpdated) { }

        public int UserId { get; set; }
        public string AvatarUrl { get; set; }
    }
}

