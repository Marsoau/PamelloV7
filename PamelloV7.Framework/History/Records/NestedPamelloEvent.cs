using System.Diagnostics;
using System.Text.Json.Serialization;
using PamelloV7.Framework.Exceptions;
using PamelloV7.Framework.DTO.Other;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Events.Base;

namespace PamelloV7.Framework.History.Records;

public class NestedPamelloEvent
{
    public static AsyncLocal<NestedPamelloEvent?> Current { get; set; } = new();
    
    private bool _isPropagated;
    
    public bool IsRevertible() => Event is RevertiblePamelloEvent { RevertPack.IsActivated: true };
    public bool IsPropagated() => _isPropagated;
    
    [JsonPropertyName("event")]
    public IPamelloEvent Event { get; set; }
    
    [JsonPropertyName("nestedEvents")]
    public List<NestedPamelloEvent> NestedEvents { get; set; }

    public NestedPamelloEvent() { }
    public NestedPamelloEvent(IPamelloEvent ev) {
        Event = ev;
        NestedEvents = [];
    }

    public bool ActivateRestorePacks(IServiceProvider services) {
        if (Event is not RevertiblePamelloEvent revertibleEvent) return false;
        if (revertibleEvent.RevertPack is null) {
            Debug.WriteLine("Restore pack is null on revertible event on activation");
            return false;
        }
        
        if (revertibleEvent.RevertPack.GetType().GetField("Services") is { } servicesProperty) {
            servicesProperty.SetValue(revertibleEvent.RevertPack, services);
        }
        if (revertibleEvent.RevertPack.GetType().GetField("Event") is { } eventProperty) {
            eventProperty.SetValue(revertibleEvent.RevertPack, Event);
        }

        foreach (var nestedEvent in NestedEvents) {
            nestedEvent.ActivateRestorePacks(services);
        }
        
        return true;
    }
    
    public void Revert(IPamelloUser scopeUser) {
        var previousNested = Current.Value;
        Current.Value = this;
        
        ((RevertiblePamelloEvent)Event).RevertPack.Revert(scopeUser);

        Propagate(scopeUser);
        
        Current.Value = previousNested;
    }

    public void Propagate(IPamelloUser scopeUser) {
        if (_isPropagated) return;
        
        foreach (var nestedRecord in NestedEvents.Where(nestedEvent => nestedEvent.IsRevertible())) {
            nestedRecord.Revert(scopeUser);
        }
        
        _isPropagated = true;
    }

    public NestedEventDTO GetDto() {
        return new NestedEventDTO() {
            EventName = Event.GetType().Name,
            EventData = Event,
            NestedEvents = NestedEvents.Select(nestedEvent => nestedEvent.GetDto()).ToList()
        };
    }
}
