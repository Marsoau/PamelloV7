using Microsoft.Extensions.DependencyInjection;
using NetCord.Gateway;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.NetCord.Extensions;

namespace PamelloV7.Module.Marsoau.NetCord.Speakers;

public class PamelloDiscordSpeakerListener : IPamelloListener
{
    public ulong DiscordId => VoiceState.UserId;
    public VoiceState VoiceState { get; set; }
    
    public IPamelloSpeaker Speaker { get; }
    public IPamelloUser? User { get; }

    public bool IsListening => !(VoiceState.IsSelfDeafened || VoiceState.IsDeafened);

    public PamelloDiscordSpeakerListener(IPamelloSpeaker speaker, VoiceState state, IServiceProvider services) {
        VoiceState = state;
        
        var users = services.GetRequiredService<IPamelloUserRepository>();
        var userTask = users.GetByDiscordId(DiscordId);
        if (!userTask.IsCompleted) {
            Output.Write("Waiting for user in discord listener constructor, this shouldnt happen", ELogLevel.Warning);
            userTask.Wait();
        }
        
        Speaker = speaker;
        User = userTask.Result;
    }
}
