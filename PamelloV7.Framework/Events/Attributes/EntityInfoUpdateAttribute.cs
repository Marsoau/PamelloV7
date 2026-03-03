namespace PamelloV7.Framework.Events.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class EntityInfoUpdateAttribute<TEntityType> : Attribute
{
    public string EntityPropertyName { get; }
    public string[] PropertyPath { get; }
    
    public EntityInfoUpdateAttribute(string entityPropertyName, params string[] propertyPath) {
        EntityPropertyName = entityPropertyName;
        PropertyPath = propertyPath;
    }
}
