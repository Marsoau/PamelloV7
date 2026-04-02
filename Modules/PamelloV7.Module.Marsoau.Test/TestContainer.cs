using PamelloV7.Core.Entities.Attributes;
using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;

namespace PamelloV7.Module.Marsoau.Test;

public abstract class TestTarget
{
    public void Method() {
        Output.Write("Test ere");
    }
}

[AutoInherit(typeof(TestTarget))]
public class TAttribute : Attribute;

[TAttribute]
public partial class TestContainer
{
    public void Test() {
        Method();
    }
}
