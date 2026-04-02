using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
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
    private readonly DiscordButtonsService _buttons;
    
    public DiscordInteractionsHandler(IServiceProvider services) {
        _services = services;
        
        _clients = services.GetRequiredService<DiscordClientService>();
        
        _commands = services.GetRequiredService<DiscordCommandsService>();
        _buttons = services.GetRequiredService<DiscordButtonsService>();
    }

    public void LateStartup() {
        _clients.Main.InteractionCreate += MainOnInteractionCreate;
    }

    private async ValueTask MainOnInteractionCreate(Interaction interaction) {
        switch (interaction) {
            case SlashCommandInteraction slashCommand:
                await _commands.ExecuteAsync(slashCommand);
                break;
            case ButtonInteraction buttonInteraction:
                await _buttons.ExecuteAsync(buttonInteraction);
                break;
        }
    }
}
