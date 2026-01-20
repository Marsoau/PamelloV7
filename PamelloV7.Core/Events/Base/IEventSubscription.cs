namespace PamelloV7.Core.Events.Base;

public interface IEventSubscription
{
    public Type EventType { get; }
    public Task Invoke(IPamelloEvent e);
}
