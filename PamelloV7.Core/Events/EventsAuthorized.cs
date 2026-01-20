using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events;

public class EventsAuthorized : IPamelloEvent
{
    public Guid UserToken { get; set; }
}
