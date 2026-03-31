using PamelloV7.Framework.Events.Base;

namespace PamelloV7.Framework.Events;

public partial class EventsAuthorized : IPamelloEvent
{
    public required Guid UserToken { get; set; }
}
