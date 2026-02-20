using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class EventsAuthorized : PamelloEvent
    {
        public EventsAuthorized() : base(EEventName.EventsAuthorized) { }

        public Guid UserToken { get; set; }
    }
}
