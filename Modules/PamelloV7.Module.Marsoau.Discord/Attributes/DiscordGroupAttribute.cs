namespace PamelloV7.Module.Marsoau.Discord.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DiscordGroupAttribute : Attribute
{
    public string GroupString { get; set; }
    public string? Description { get; set; }
    
    public DiscordGroupAttribute(string group, string? description = null) {
        GroupString = group;
        Description = description;
    }
}
