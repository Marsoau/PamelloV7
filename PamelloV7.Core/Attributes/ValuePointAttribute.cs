namespace PamelloV7.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ValuePointAttribute : Attribute
{
    public string[] Names { get; }
    
    public ValuePointAttribute(string name) {
        Names = [name];
    }
    public ValuePointAttribute(string[] names) {
        Names = names;
    }
    
    public bool Is(string name) => Names.Contains(name);
}
