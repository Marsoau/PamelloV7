using PamelloV7.Module.Marsoau.Discord.Attributes;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;

public class PlayerGroupAttribute : DiscordGroupAttribute {
    public PlayerGroupAttribute() : base("player", "Actions with players") { }
}

public class PlayerQueueGroupAttribute : DiscordGroupAttribute {
    public PlayerQueueGroupAttribute() : base("player queue", "Actions with selected player queue") { }
}
