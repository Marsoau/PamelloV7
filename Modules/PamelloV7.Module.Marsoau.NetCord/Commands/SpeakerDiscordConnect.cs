using Microsoft.Extensions.DependencyInjection;
using NetCord.Gateway.Voice;
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

        var priorityDiscordUserId = 0ul;
        
        if (discordUserId == 0 && (
            ScopeUser.GetPriorityPlatformKey("discord") is not { } discordIdStr ||
            !ulong.TryParse(discordIdStr, out priorityDiscordUserId)
        )) throw new PamelloException("Could not find discord id for your user");
        
        var voiceState = clients.GetUserVoiceState(discordUserId);

        if (voiceState is null) {
            foreach (var authorization in ScopeUser.Authorizations) {
                if (authorization.PK.Platform != "discord") continue;
                if (ulong.TryParse(authorization.PK.Key, out var discordId)) discordUserId = discordId;
            
                if (discordUserId == priorityDiscordUserId) continue;
                
                voiceState = clients.GetUserVoiceState(discordUserId);
                if (voiceState is not null) break;
            }
        }
        
        if (voiceState?.ChannelId is null) throw new PamelloException("You have to be in a voice channel to connect a speaker");
        
        var availableClient = clients.GetAvailableClient(discordUserId);
        if (availableClient is null) throw new PamelloException("No available clients to connect");
        
        var speakers = Services.GetRequiredService<IPamelloSpeakerRepository>();
        var speaker = new PamelloDiscordSpeaker(speakers.NextId, ScopeUser.GuaranteedSelectedPlayer, Services);

        await speaker.ConnectAsync(voiceState.GuildId, voiceState.ChannelId.Value);
        
        return speakers.Add(speaker);
    }
}
