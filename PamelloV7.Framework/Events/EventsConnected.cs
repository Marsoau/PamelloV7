using PamelloV7.Framework.Events.Base;

namespace PamelloV7.Framework.Events;

public partial class EventsConnected : IPamelloEvent
{
    public required Guid EventsToken { get; set; }
}
