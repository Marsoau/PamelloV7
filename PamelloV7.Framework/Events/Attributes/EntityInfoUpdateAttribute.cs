namespace PamelloV7.Framework.Events.Attributes;

public interface IEntityInfoUpdateAttribute
{
    public Type EntityType { get; }
    public string EntityPropertyName { get; }
    public string[] PropertyPath { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class EntityInfoUpdateAttribute<TEntityType> : Attribute, IEntityInfoUpdateAttribute
{
    public Type EntityType => typeof(TEntityType);
    public string EntityPropertyName { get; }
    public string[] PropertyPath { get; }
    
    public EntityInfoUpdateAttribute(string entityPropertyName, params string[] propertyPath) {
        EntityPropertyName = entityPropertyName;
        PropertyPath = propertyPath;
    }
}
