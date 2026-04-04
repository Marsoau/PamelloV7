using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Commands.Base;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.NetCord.Services;
using PamelloV7.Module.Marsoau.NetCord.Speakers;

namespace PamelloV7.Module.Marsoau.NetCord.Commands;

public class SpeakerDiscordConnect : PamelloCommand
{
    public async Task<PamelloDiscordSpeaker> Execute() {
        var clients = Services.GetRequiredService<DiscordClientService>();
        
        var vc = clients.GetUserVoiceState(ScopeUser);
        if (vc?.ChannelId is null) throw new PamelloException("You have to be in a voice channel to connect a speaker");
        
        var speakers = Services.GetRequiredService<IPamelloSpeakerRepository>();
        var speaker = new PamelloDiscordSpeaker(speakers.NextId, vc.GuildId, ScopeUser.GuaranteedSelectedPlayer, Services);

        await speaker.ConnectAsync(vc.ChannelId.Value);
        
        return speakers.Add(speaker);
    }
}
