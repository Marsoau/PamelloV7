using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Repositories;
using PamelloV7.Module.Marsoau.NetCord.Extensions;

namespace PamelloV7.Module.Marsoau.NetCord.Speakers;

public class PamelloDiscordSpeakerListener : IPamelloListener
{
    public ulong DiscordId { get; }
    public IPamelloSpeaker Speaker { get; }
    public IPamelloUser? User { get; }

    public PamelloDiscordSpeakerListener(IPamelloSpeaker speaker, ulong discordUserId, IServiceProvider services) {
        var users = services.GetRequiredService<IPamelloUserRepository>();
        var userTask = users.GetByDiscordId(discordUserId);
        if (!userTask.IsCompleted) {
            Output.Write("Waiting for user in discord listener constructor, this shouldnt happen", ELogLevel.Warning);
            userTask.Wait();
        }
        
        DiscordId = discordUserId;
        Speaker = speaker;
        User = userTask.Result;
    }
}
