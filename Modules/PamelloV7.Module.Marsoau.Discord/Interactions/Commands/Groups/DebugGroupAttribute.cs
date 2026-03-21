using PamelloV7.Module.Marsoau.Discord.Attributes;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;

public class DebugGroupAttribute : DiscordGroupAttribute {
    public DebugGroupAttribute() : base("debug", "Debug commands") { }
}
