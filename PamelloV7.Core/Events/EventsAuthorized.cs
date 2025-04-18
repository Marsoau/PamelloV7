using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.Events
{
    public class EventsAuthorized : PamelloEvent
    {
        public EventsAuthorized() : base(EEventName.EventsAuthorized) { }

        public Guid UserToken { get; set; }
    }
}
