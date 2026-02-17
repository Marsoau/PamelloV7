using PamelloV7.Core.Entities;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Exceptions;

namespace PamelloV7.Core.History.Records;

public class NestedPamelloEvent
{
    public bool IsRevertible() => Event is RevertiblePamelloEvent { RevertPack.IsActivated: true };
    
    public IPamelloEvent Event { get; set; }
    public List<NestedPamelloEvent> NestedEvents { get; set; }

    public NestedPamelloEvent() { }
    public NestedPamelloEvent(IPamelloEvent ev) {
        Event = ev;
        NestedEvents = [];
    }

    public bool ActivateRestorePacks(IServiceProvider services) {
        if (Event is not RevertiblePamelloEvent revertibleEvent) return false;
        
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
        ((RevertiblePamelloEvent)Event).RevertPack.Revert(scopeUser);

        foreach (var nestedRecord in NestedEvents.Where(nestedEvent => nestedEvent.IsRevertible())) {
            nestedRecord.Revert(scopeUser);
        }
    }
}
