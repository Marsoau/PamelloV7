namespace PamelloV7.Framework.Events.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DtoInfoUpdateAttribute : Attribute
{
    public Type EntityType { get; }
    public string[] PropertyPath { get; }
    
    public DtoInfoUpdateAttribute(Type entityName, params string[] propertyPath) {
        EntityType = entityName;
        PropertyPath = propertyPath;
    }
}
