using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Services;

public class DiscordButtonsService : IPamelloService
{
    private readonly IServiceProvider _services;
    
    private readonly UpdatableMessageService _messages;
    
    public DiscordButtonsService(IServiceProvider services) {
        _services = services;
        
        _messages = services.GetRequiredService<UpdatableMessageService>();
    }
    
    public async Task ExecuteAsync(ButtonInteraction interaction) {
        if (interaction.Data.CustomId == "refresh") {
            await Refresh(interaction);
            return;
        }
        
    }

    private async Task Refresh(ButtonInteraction interaction) {
        if (interaction.Message.InteractionMetadata is null) return;

        var message = _messages.Get(interaction.Message.InteractionMetadata.Id);
        if (message is null) return;
        
        await message.Refresh();
        
        await interaction.SendResponseAsync(InteractionCallback.ModifyMessage(_ => { }));
    }
}
