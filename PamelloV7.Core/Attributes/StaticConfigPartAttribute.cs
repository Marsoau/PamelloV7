namespace PamelloV7.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class StaticConfigPartAttribute : Attribute
{
    public string Name { get; }

    public StaticConfigPartAttribute(string name) {
        Name = name;
    }
}
