using PamelloV7.Framework.Entities;

namespace PamelloV7.Framework.Events.Base;

public interface IPamelloEvent
{
    public IPamelloUser? Invoker { get; set; }
}
