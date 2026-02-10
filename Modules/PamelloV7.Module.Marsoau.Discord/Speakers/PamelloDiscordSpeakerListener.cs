using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Other;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Repositories;

namespace PamelloV7.Module.Marsoau.Discord.Speakers;

public class PamelloDiscordSpeakerListener : IPamelloListener
{
    private readonly IPamelloUserRepository _users;
    
    public SocketUser DiscordUser { get; }
    public IPamelloSpeaker Speaker { get; }
    public IPamelloUser? User => _users.GetByPlatformKey(new PlatformKey("discord", DiscordUser.Id.ToString()));

    public PamelloDiscordSpeakerListener(SocketUser discordUser, IPamelloSpeaker speaker, IServiceProvider services) {
        _users = services.GetRequiredService<IPamelloUserRepository>();
        
        DiscordUser = discordUser;
        Speaker = speaker;
    }
}
