using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class UserCreated : PamelloEvent
    {
        public UserCreated() : base(EEventName.UserCreated) { }
    }
}
