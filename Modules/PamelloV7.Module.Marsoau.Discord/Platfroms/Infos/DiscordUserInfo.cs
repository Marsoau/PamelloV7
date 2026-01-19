using Discord.WebSocket;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.Discord.Platfroms.Infos;

public class DiscordUserInfo : IUserInfo
{
    public IUserPlatform Platform { get; }
    
    public SocketUser User { get; }
    
    public string Key => User.Id.ToString();
    public string Name => User.GlobalName;
    public string AvatarUrl => User.GetAvatarUrl();

    public DiscordUserInfo(IUserPlatform platform, SocketUser user) {
        Platform = platform;
        User = user;
    }
    
    public override string ToString() => $"{Name} ({Key})";
}
