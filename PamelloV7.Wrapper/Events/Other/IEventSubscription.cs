using PamelloV7.Wrapper.Events.Base;
using PamelloV7.Wrapper.Events.Dto;

namespace PamelloV7.Wrapper.Events.Other;

public interface IEventSubscription
{
    public Type EventType { get; }

    public void Invoke(IRemoteEvent e);
}
