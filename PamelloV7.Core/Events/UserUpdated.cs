using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class UserUpdated : PamelloEvent
    {
        public UserUpdated() : base(EEventName.UserUpdated) { }
    }
}

