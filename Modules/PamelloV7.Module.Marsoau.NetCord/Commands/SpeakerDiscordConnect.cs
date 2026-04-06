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
    public async Task<PamelloDiscordSpeaker> Execute(ulong discordUserId = 0) {
        var clients = Services.GetRequiredService<DiscordClientService>();
        
        if (
            discordUserId == 0 && (
                ScopeUser.GetPriorityPlatformKey("discord") is not { } discordIdStr ||
                !ulong.TryParse(discordIdStr, out discordUserId)
            )
        ) throw new PamelloException("Could not find discord id for your user");
        
        var vc = clients.GetUserVoiceState(discordUserId);
        if (vc?.ChannelId is null) throw new PamelloException("You have to be in a voice channel to connect a speaker");
        
        var speakers = Services.GetRequiredService<IPamelloSpeakerRepository>();
        var speaker = new PamelloDiscordSpeaker(speakers.NextId, vc.GuildId, ScopeUser.GuaranteedSelectedPlayer, Services);

        await speaker.ConnectAsync(vc.ChannelId.Value);
        
        return speakers.Add(speaker);
    }
}
