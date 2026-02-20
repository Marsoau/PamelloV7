using System.Text.Json.Serialization;
using PamelloV7.Framework.Events.RestorePacks.Base;
using PamelloV7.Framework.Events.RestorePacks;

namespace PamelloV7.Framework.Events.Base;

public abstract class RevertiblePamelloEvent
{
    [JsonIgnore]
    public IRevertPack RevertPack { get; set; }
}
