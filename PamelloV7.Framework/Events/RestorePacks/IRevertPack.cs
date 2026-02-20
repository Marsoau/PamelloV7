using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Base;

namespace PamelloV7.Framework.Events.RestorePacks;

public interface IRevertPack
{
    public IPamelloEvent Event { get; }
    public bool IsActivated { get; }
    
    public void Revert(IPamelloUser scopeUser);
}
