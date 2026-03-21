using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;

public class UserGroupAttribute : DiscordGroupAttribute {
    public UserGroupAttribute() : base("user", "Actions with users") { }
}
