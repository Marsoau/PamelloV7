namespace PamelloV7.Module.Marsoau.NetCord.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DiscordCommandAttribute : Attribute
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public DiscordCommandAttribute(string name, string description) {
        Name = name;
        Description = description;
    }
}
