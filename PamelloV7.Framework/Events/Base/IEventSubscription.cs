using PamelloV7.Framework.History.Records;
using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Events.Base;

public interface IEventSubscription
{
    public Type EventType { get; }

    public void Invoke(IPamelloEvent e);
}
