namespace PamelloV7.Core.Entities.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SafeEntityAttribute<TEntityType> : Attribute
    where TEntityType : class
{
    public SafeEntityAttribute(string name, params Type[] attributeTypes) { }
}
