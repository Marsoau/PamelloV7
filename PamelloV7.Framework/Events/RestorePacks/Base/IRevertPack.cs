using System.Text.Json.Serialization;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.History.Records;

namespace PamelloV7.Framework.Events.RestorePacks.Base;

public interface IRevertPack
{
    [JsonIgnore]
    public IPamelloEvent Event { get; }
    public bool IsInitialized { get; }

    public void InitializePack(IServiceProvider services, IPamelloEvent eventInstance, NestedPamelloEvent? nestedEvent = null);
    public void Revert(IPamelloUser scopeUser);
}
