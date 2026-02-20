namespace PamelloV7.Framework.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class StaticConfigPartAttribute : Attribute
{
    public string? Name { get; }

    public StaticConfigPartAttribute() { } 
    public StaticConfigPartAttribute(string name) {
        Name = name;
    }
}
