using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class UserNameUpdated : PamelloEvent
    {
        public UserNameUpdated() : base(EEventName.UserNameUpdated) { }

        public int UserId { get; set; }
        public string Name { get; set; }
    }
}

