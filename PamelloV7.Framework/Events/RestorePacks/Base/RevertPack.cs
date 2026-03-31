using System.Text.Json.Serialization;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Exceptions;

namespace PamelloV7.Framework.Events.RestorePacks.Base;

public abstract class RevertPack<TEventType> : IRevertPack
    where TEventType : class, IRevertiblePamelloEvent, IPamelloEvent
{
    public bool IsExpired { get; set; }
    
    public readonly IServiceProvider Services = null!;
    public readonly TEventType Event = null!;
    
    [JsonIgnore]
    IPamelloEvent IRevertPack.Event => Event;
    
    public bool IsActivated => Services is not null && Event is not null;
    
    public void Revert(IPamelloUser scopeUser) {
        if (!IsActivated) throw new PamelloException("Revert pack is not activated yet");
        if (!DidNotExpire(scopeUser)) throw new PamelloException("Revert pack has expired");
        
        RevertInternal(scopeUser);
    }

    public bool DidNotExpire(IPamelloUser scopeUser) {
        if (IsExpired) return true;
        return DidNotExpireInternal(scopeUser);
    }

    protected abstract void RevertInternal(IPamelloUser scopeUser);
    protected virtual bool DidNotExpireInternal(IPamelloUser scopeUser) => false;
}
