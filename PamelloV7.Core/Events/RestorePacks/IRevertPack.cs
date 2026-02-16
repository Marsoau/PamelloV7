using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Base;

namespace PamelloV7.Core.Events.RestorePacks;

public interface IRevertPack
{
    public IPamelloEvent Event { get; }
    public bool IsActivated { get; }
    
    public void Revert(IPamelloUser scopeUser);
}
