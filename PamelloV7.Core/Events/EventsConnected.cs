using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events;

public class EventsConnected : IPamelloEvent
{
    public Guid EventsToken { get; set; }
}
