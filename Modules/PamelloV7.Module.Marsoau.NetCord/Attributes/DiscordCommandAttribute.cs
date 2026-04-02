using PamelloV7.Framework.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Attributes;

public record SlashCommandNameInfo(
    string Command,
    string? SubGroup,
    string? SubCommand,
    string Description,
    DiscordCommandAttribute Attribute
);

[AutoInherit(typeof(DiscordCommand))]
[RequiredMethodName("Execute")]

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DiscordCommandAttribute : Attribute
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public DiscordCommandAttribute(string name, string description) {
        Name = name;
        Description = description;
    }

    public SlashCommandNameInfo GetInfo() {
        var parts = Name.Split(' ');
        
        var name = parts.First();
        string? subGroup = null;
        string? subCommand = null;

        switch (parts.Length) {
            case 1:
                break;
            case 2:
                subCommand = parts[1];
                break;
            case 3:
                subGroup = parts[1];
                subCommand = parts[2];
                break;
            default:
                throw new Exception($"Invalid command name \"{Name}\", too many spaces");
        }

        return new SlashCommandNameInfo(
            name, subGroup, subCommand, Description, this
        );
    }
}
