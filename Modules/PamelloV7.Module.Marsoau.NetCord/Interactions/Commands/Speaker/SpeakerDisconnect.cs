using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Speaker;

[DiscordCommand("speaker disconnect", "Disconnect a speaker")]
public partial class SpeakerDisconnect
{
    public async Task Execute(
        [Description("speaker", "Speaker to disconnect")]
        [DefaultQuery("current")]
        IPamelloSpeaker speaker
    ) {
        Command<Framework.Commands.SpeakerDisconnect>().Execute(speaker);
        
        await RespondAsync("Speaker Disconnected");
    }
}
