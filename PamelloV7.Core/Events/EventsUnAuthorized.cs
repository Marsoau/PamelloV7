using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class EventsUnAuthorized : PamelloEvent
    {
        public EventsUnAuthorized() : base(EEventName.EventsUnAuthorized) { }
    }
}
