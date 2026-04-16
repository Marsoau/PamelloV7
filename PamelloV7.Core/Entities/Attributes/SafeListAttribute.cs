namespace PamelloV7.Core.Entities.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SafeListAttribute<TEntityType> : Attribute
    where TEntityType : class
{
    public SafeListAttribute(string name, bool isRequired = false, params Type[] attributeTypes) { }
}
