namespace PamelloV7.Core.Entities.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SafeAttribute<TEntityType> : Attribute
    where TEntityType : class
{
    public SafeAttribute(string name, bool isRequired = false, params Type[] attributeTypes) { }
}
