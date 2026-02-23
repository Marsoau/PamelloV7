using PamelloV7.Framework.Entities.Base;

namespace PamelloV7.Framework.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SafeStoredAttribute<TEntityType> : Attribute
    where TEntityType : class, IPamelloEntity
{
    public SafeStoredAttribute(string name) { }
}
