using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class EventsAuthorized : PamelloEvent
    {
        public EventsAuthorized() : base(EEventName.EventsAuthorized) { }

        public Guid UserToken { get; set; }
    }
}
