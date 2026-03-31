using Microsoft.Extensions.DependencyInjection;
using NetCord;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.General;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Handlers;

public class DiscordInteractionsHandler : IPamelloService
{
    private readonly IServiceProvider _services;
    
    private readonly DiscordClientService _clients;
    
    private readonly DiscordCommandsService _commands;
    
    public DiscordInteractionsHandler(IServiceProvider services) {
        _services = services;
        
        _clients = services.GetRequiredService<DiscordClientService>();
        
        _commands = services.GetRequiredService<DiscordCommandsService>();
    }

    public void LateStartup() {
        _clients.Main.InteractionCreate += MainOnInteractionCreate;
    }

    private async ValueTask MainOnInteractionCreate(Interaction interaction) {
        if (interaction is not SlashCommandInteraction slashCommand) return;

        await _commands.ExecuteAsync(slashCommand);
    }
}
