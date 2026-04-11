using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Messages;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons;

[DiscordButton("Prev")]
public partial class PrevButton
{
    public async Task Execute() {
        if (Message is not UpdatablePageMessage pageMessage) return;
        
        await pageMessage.SetPage(pageMessage.Page - 1);
    }
}
