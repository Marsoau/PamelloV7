using System.Text.Json.Serialization;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Base;

namespace PamelloV7.Framework.Events.RestorePacks.Base;

public interface IRevertPack
{
    [JsonIgnore]
    public IPamelloEvent Event { get; }
    public bool IsActivated { get; }
    
    public void Revert(IPamelloUser scopeUser);
}
