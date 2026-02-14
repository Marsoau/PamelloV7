using System.Text.Json.Serialization;

namespace PamelloV7.Core.Events.RestorePacks.Base;

public abstract class RevertPack
{
    [JsonIgnore]
    public readonly IServiceProvider Services;
    
    public abstract void Revert();
}
