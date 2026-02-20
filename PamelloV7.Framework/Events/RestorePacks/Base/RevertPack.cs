using System.Text.Json.Serialization;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Exceptions;

namespace PamelloV7.Framework.Events.RestorePacks.Base;

public abstract class RevertPack<TEventType> : IRevertPack
    where TEventType : RevertiblePamelloEvent, IPamelloEvent
{
    public bool IsExpired { get; set; }
    
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

    public bool DidExpire() {
        if (IsExpired) return true;
        return DidExpireInternal();
    }

    protected abstract void RevertInternal(IPamelloUser scopeUser);
    protected virtual bool DidExpireInternal() => false;
}
