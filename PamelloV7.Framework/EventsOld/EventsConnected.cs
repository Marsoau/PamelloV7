using PamelloV7.Framework.Enumerators;

namespace PamelloV7.Framework.EventsOld
{
    public class EventsConnected : PamelloEvent
    {
        public EventsConnected() : base(EEventName.EventsConnected) { }

        public Guid EventsToken { get; set; }
    }
}
