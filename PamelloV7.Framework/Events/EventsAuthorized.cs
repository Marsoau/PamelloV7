using PamelloV7.Framework.Events.Base;

namespace PamelloV7.Framework.Events;

public class EventsAuthorized : IPamelloEvent
{
    public Guid UserToken { get; set; }
}
