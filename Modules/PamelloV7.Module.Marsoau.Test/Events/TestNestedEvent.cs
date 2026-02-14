using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;

namespace PamelloV7.Module.Marsoau.Test.Events;

[Broadcast]
public class TestNestedEvent : IPamelloEvent
{
    public int Value { get; set; }
}

