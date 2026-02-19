using PamelloV7.Core.Events.Attributes;
using PamelloV7.Core.Events.Base;
using PamelloV7.Core.Events.Enumerators;

namespace PamelloV7.Module.Marsoau.Test.Events;

[Broadcast]
[HistoricalEvent]
[PamelloEventCategory(EEventCategory.Miscellaneous)]
public class TestEvent : IPamelloEvent
{
    public int Value { get; set; }
}

