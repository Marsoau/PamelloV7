using System.Text.Json.Serialization;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.History.Records;

namespace PamelloV7.Framework.Events.RestorePacks.Base;

public abstract class RevertPack<TEventType> : IRevertPack
    where TEventType : class, IRevertiblePamelloEvent, IPamelloEvent
{
    public bool IsExpired { get; set; }
    
    public IServiceProvider Services = null!;
    public TEventType Event = null!;

    public NestedPamelloEvent? NestedEvent;
    
    [JsonIgnore]
    IPamelloEvent IRevertPack.Event => Event;
    
    public bool IsInitialized => Services is not null && Event is not null;

    public void InitializePack(
        IServiceProvider services, IPamelloEvent eventInstance, NestedPamelloEvent? nestedEvent = null
    ) {
        if (eventInstance is not TEventType typedEvent) throw new PamelloException("Event is not of the correct type");

        InitializePack(services, typedEvent, nestedEvent);
    }
    
    public void InitializePack(IServiceProvider services, TEventType eventInstance, NestedPamelloEvent? nestedEvent = null) {
        NestedEvent = nestedEvent;
        
        if (IsInitialized) return;
        
        Services = services;
        Event = eventInstance;
    }

    public void Revert(IPamelloUser scopeUser) {
        if (!IsInitialized) throw new PamelloException("Revert pack is not activated yet");
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
