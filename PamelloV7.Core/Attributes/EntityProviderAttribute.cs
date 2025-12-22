namespace PamelloV7.Core.Attributes;

[AttributeUsage(AttributeTargets.Interface)]
public class EntityProviderAttribute : Attribute
{
    public string Name { get; }
    
    public EntityProviderAttribute(string name) {
        Name = name;
    }
}
