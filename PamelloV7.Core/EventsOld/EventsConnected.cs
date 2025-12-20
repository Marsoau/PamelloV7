using PamelloV7.Core.Enumerators;

namespace PamelloV7.Core.EventsOld
{
    public class EventsConnected : PamelloEvent
    {
        public EventsConnected() : base(EEventName.EventsConnected) { }

        public Guid EventsToken { get; set; }
    }
}
