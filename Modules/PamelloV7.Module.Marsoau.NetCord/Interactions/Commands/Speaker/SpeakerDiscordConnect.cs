using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Speaker;

[DiscordCommand("speaker discord-connect", "Connect to discord")]
public partial class SpeakerDiscordConnect
{
    public async Task Execute() {
        await Command<Marsoau.NetCord.Commands.SpeakerDiscordConnect>().Execute();
        
        await RespondAsync("executed");
    }
}
