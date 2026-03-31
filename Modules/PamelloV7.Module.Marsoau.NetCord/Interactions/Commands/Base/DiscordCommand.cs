using NetCord;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;

public abstract class DiscordCommand
{
    public readonly IServiceProvider Services = null!;
    
    public readonly SlashCommandInteraction Interaction = null!;
}
