using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class UserCreated : PamelloEvent
    {
        public UserCreated() : base(EEventName.UserCreated) { }
    }
}
