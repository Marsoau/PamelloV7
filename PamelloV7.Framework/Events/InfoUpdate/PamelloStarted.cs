using PamelloV7.Framework.Events.Base;

namespace PamelloV7.Framework.Events.InfoUpdate;

public partial class PamelloStarted : IPamelloEvent
{
    public IServiceProvider Services { init; get; }
}
