using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events;

public class PamelloStarted : IPamelloEvent
{
    public IServiceProvider Services { init; get; }
}
