using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Exceptions;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;
using PamelloV7.Module.Marsoau.Discord.Platfroms.Infos;
using PamelloV7.Module.Marsoau.Discord.Services;

namespace PamelloV7.Module.Marsoau.Discord.Platfroms;

public class DiscordUserPlatform : IUserPlatform
{
    private readonly DiscordClientService _clients;
    
    public string Name => "discord";
    public string IconUrl => "";
    
    public DiscordUserPlatform(IServiceProvider services) {
        _clients = services.GetRequiredService<DiscordClientService>();
    }
    
    public string ValueToKey(string value) {
        ulong id;
        
        if (value.StartsWith("<@") && value.EndsWith('>')) {
            if (ulong.TryParse(value[2..^1], out id)) return id.ToString();
        }
        
        throw new PamelloException($"Cannot find discord id in value \"{value}\"");
    }

    public IUserInfo? GetUserInfo(string key) {
        if (!ulong.TryParse(key, out var discordId)) return null;
        
        var user = _clients.GetDiscordUser(discordId);
        if (user is null) return null;
        
        return new DiscordUserInfo(this, user);
    }
}
