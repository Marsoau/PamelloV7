using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class UserCreated : PamelloEvent
    {
        public UserCreated() : base(EEventName.UserCreated) { }
    }
}
