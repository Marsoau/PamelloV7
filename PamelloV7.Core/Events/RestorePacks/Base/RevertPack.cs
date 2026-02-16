using System.Text.Json.Serialization;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Core.Events.RestorePacks.Base;

public abstract class RevertPack<TEventType> : IRevertPack
    where TEventType : RevertiblePamelloEvent, IPamelloEvent
{
    [JsonIgnore]
    public readonly IServiceProvider Services;
    [JsonIgnore]
    public readonly TEventType Event;
    
    IPamelloEvent IRevertPack.Event => Event;
    
    public bool IsActivated => Services is not null && Event is not null;
    
    public void Revert(IPamelloUser scopeUser) {
        if (!IsActivated) throw new PamelloException("Revert pack is not activated yet");
        
        RevertInternal(scopeUser);
    }

    protected abstract void RevertInternal(IPamelloUser scopeUser);
}
