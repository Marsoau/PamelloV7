using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Base;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons;

[DiscordButton("Refresh")]
public partial class RefreshButton
{
    public async Task Execute() {
        await Message.Refresh();
    }
}
