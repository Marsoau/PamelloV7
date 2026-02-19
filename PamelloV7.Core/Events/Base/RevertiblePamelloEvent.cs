using System.Text.Json.Serialization;
using PamelloV7.Core.Events.RestorePacks;
using PamelloV7.Core.Events.RestorePacks.Base;

namespace PamelloV7.Core.Events.Base;

public abstract class RevertiblePamelloEvent
{
    [JsonIgnore]
    public IRevertPack RevertPack { get; set; }
}
