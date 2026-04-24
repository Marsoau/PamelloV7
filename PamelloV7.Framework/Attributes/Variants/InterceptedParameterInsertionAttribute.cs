namespace PamelloV7.Framework.Attributes.Variants;

[AttributeUsage(AttributeTargets.Method)]
public class InterceptedParameterInsertionAttribute : Attribute
{
    public InterceptedParameterInsertionAttribute(int index, int replaceCount) {}
}
