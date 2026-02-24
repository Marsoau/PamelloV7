using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SafeEntityAttribute<TEntityType> : Attribute
    where TEntityType : class, IPamelloEntity
{
    public SafeEntityAttribute(string name, params Type[] attributeTypes) { }
}
