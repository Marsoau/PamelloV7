namespace PamelloV7.Core.Entities.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SafeEntitiesAttribute<TEntityType> : Attribute
    where TEntityType : class
{
    public SafeEntitiesAttribute(string name, params Type[] attributeTypes) { }
}
