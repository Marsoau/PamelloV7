using PamelloV7.Framework.Events.Base;

namespace PamelloV7.Framework.Events;

public class EventsConnected : IPamelloEvent
{
    public Guid EventsToken { get; set; }
}
