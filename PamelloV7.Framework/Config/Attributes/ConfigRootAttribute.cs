namespace PamelloV7.Framework.Config.Attributes;

public class ConfigRootAttribute : Attribute
{
    public string? Name { get; }

    public ConfigRootAttribute() { } 
    public ConfigRootAttribute(string name) {
        Name = name;
    }
}
