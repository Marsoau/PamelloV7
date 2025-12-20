namespace PamelloV7.Core.Events.Base;

public interface IEventSubscription
{
    public Type Type { get; }
    public Task Invoke(IPamelloEvent e);
}
