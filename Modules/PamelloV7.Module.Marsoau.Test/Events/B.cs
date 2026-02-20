using PamelloV7.Framework.Events.Attributes;
using PamelloV7.Framework.Events.Base;
using PamelloV7.Framework.Events.Enumerators;

namespace PamelloV7.Module.Marsoau.Test.Events;

[PamelloEventCategory(EEventCategory.Miscellaneous)]
public partial class B : A, IPamelloEvent
{
    
}

