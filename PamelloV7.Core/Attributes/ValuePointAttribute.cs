namespace PamelloV7.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ValuePointAttribute : Attribute
{
    public string Name { get; }
    
    public ValuePointAttribute(string name) {
        Name = name;
    }
}
