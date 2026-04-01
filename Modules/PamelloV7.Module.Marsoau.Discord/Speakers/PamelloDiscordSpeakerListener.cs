using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Entities.Other;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Repositories;

namespace PamelloV7.Module.Marsoau.Discord.Speakers;

public class PamelloDiscordSpeakerListener : IPamelloListener
{
    private readonly IPamelloUserRepository _users;
    
    public SocketUser DiscordUser { get; }
    public IPamelloSpeaker Speaker { get; }

    public IPamelloUser? User {
        get => field ?? _users.GetByPlatformKey(new PlatformKey("discord", DiscordUser.Id.ToString())).Result;
        private init => field = value;
    }

    public PamelloDiscordSpeakerListener(SocketUser discordUser, IPamelloSpeaker speaker, IServiceProvider services) {
        _users = services.GetRequiredService<IPamelloUserRepository>();
        User = null;
        
        DiscordUser = discordUser;
        Speaker = speaker;
    }
}
