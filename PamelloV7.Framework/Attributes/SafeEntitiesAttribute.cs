using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SafeEntitiesAttribute<TEntityType> : Attribute
    where TEntityType : class, IPamelloEntity
{
    public SafeEntitiesAttribute(string name, params Type[] attributeTypes) { }
}
