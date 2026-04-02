using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Base;
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
    
    private readonly InteractionTokenizationService _tokenizer;
    
    public DiscordInteractionsHandler(IServiceProvider services) {
        _services = services;
        
        _clients = services.GetRequiredService<DiscordClientService>();
        
        _commands = services.GetRequiredService<DiscordCommandsService>();
        _buttons = services.GetRequiredService<DiscordButtonsService>();
        
        _tokenizer = services.GetRequiredService<InteractionTokenizationService>();
    }

    public void LateStartup() {
        _clients.Main.InteractionCreate += MainOnInteractionCreate;
    }

    private async ValueTask MainOnInteractionCreate(Interaction interaction) {
        switch (interaction) {
            case SlashCommandInteraction slashCommand:
                await _commands.ExecuteAsync(slashCommand);
                break;
            case ButtonInteraction buttonInteraction when buttonInteraction.Data.CustomId.StartsWith("tokenized:"):
                var tokenizedInteraction = _tokenizer.GetRequired(buttonInteraction);
                if (tokenizedInteraction is ITokenizedButtonInteraction tokenizedButtonInteraction) {
                    var button = await tokenizedButtonInteraction.ExecuteButtonAsync(buttonInteraction);

                    await button.ReleaseInteraction();
                    
                    break;
                }
                
                await tokenizedInteraction.Action(interaction);
                await buttonInteraction.SendResponseAsync(InteractionCallback.ModifyMessage(_ => { }));
                
                break;
        }
    }
}
