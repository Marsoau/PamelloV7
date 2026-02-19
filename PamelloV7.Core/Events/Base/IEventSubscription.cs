using PamelloV7.Core.Entities;
using PamelloV7.Core.History.Records;

namespace PamelloV7.Core.Events.Base;

public interface IEventSubscription
{
    public Type EventType { get; }

    public void Invoke(IPamelloUser? scopeUser, IPamelloEvent e);
}
