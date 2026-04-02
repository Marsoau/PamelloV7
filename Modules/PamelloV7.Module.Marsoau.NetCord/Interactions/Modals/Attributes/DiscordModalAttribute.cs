namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DiscordModalAttribute : Attribute
{
    public string Name { get; set; }
    
    public DiscordModalAttribute(string name) {
        Name = name;
    }
}
