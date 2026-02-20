using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class UserNameUpdated : PamelloEvent
    {
        public UserNameUpdated() : base(EEventName.UserNameUpdated) { }

        public int UserId { get; set; }
        public string Name { get; set; }
    }
}

