using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.NetCord.Services;

namespace PamelloV7.Module.Marsoau.NetCord.Commands;

public class SpeakerDiscordConnect : PamelloCommand
{
    public async Task Execute() {
        var clients = Services.GetRequiredService<DiscordClientService>();
        
        var vcId = clients.GetUserVoiceChannelId(ScopeUser);
        if (vcId is null) throw new PamelloException("You have to be in a voice channel to connect a speaker");

        Output.Write($"vc id was: {vcId}");
        
        /*
        var speakers = Services.GetRequiredService<IPamelloSpeakerRepository>();
        var speaker = new PamelloDiscordSpeaker(speakers.NextId, vc.Guild.Id, ScopeUser.GuaranteedSelectedPlayer, Services);

        await speaker.ConnectAsync(vc.Id);
        
        return speakers.Add(speaker);
        */
    }
}
