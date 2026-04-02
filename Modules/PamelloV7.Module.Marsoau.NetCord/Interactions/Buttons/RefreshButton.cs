using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons;

[DiscordButton("Refresh")]
public class RefreshButton : DiscordButton
{
    public async Task Execute() {
        if (Interaction.Message.InteractionMetadata is null) return;
        
        var messages = Services.GetRequiredService<UpdatableMessageService>();

        var message = messages.Get(Interaction.Message.InteractionMetadata.Id);
        if (message is null) return;
        
        await message.Refresh();
    }
}
