using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Repositories;
using PamelloV7.Module.Marsoau.Discord.Services;
using PamelloV7.Module.Marsoau.Discord.Speakers;

namespace PamelloV7.Module.Marsoau.Discord.Commands;

public class SpeakerDiscordConnect : PamelloCommand
{
    public async Task<PamelloDiscordSpeaker> Execute() {
        var clients = Services.GetRequiredService<DiscordClientService>();
        
        var vc = clients.GetUserVoiceChannel(ScopeUser);
        if (vc is null) throw new PamelloException("You have to be in a voice channel to connect a speaker");
        
        var speakers = Services.GetRequiredService<IPamelloSpeakerRepository>();
        var speaker = new PamelloDiscordSpeaker(speakers.NextId, vc.Guild.Id, ScopeUser.GuaranteedSelectedPlayer, Services);

        await speaker.ConnectAsync(vc.Id);
        
        return speakers.Add(speaker);
    }
}
