using PamelloV7.Core.Events.RestorePacks.Base;

namespace PamelloV7.Core.Events.Base;

public abstract class RestorablePamelloEvent
{
    public RevertPack RevertPack { get; set; }
}
