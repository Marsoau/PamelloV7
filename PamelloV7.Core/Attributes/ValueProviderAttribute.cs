namespace PamelloV7.Core.Attributes;

[AttributeUsage(AttributeTargets.Interface)]
public class ValueProviderAttribute : Attribute
{
    public string Name { get; }
    
    public ValueProviderAttribute(string name) {
        Name = name;
    }
}
