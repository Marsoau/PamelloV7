using NetCord;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.Discord.Platfroms.Infos;

public class DiscordUserInfo : IUserInfo
{
    public IUserPlatform Platform { get; }
    
    public User User { get; }
    
    public string Key => User.Id.ToString();
    public string Name => User.GlobalName ?? User.Username;
    public string AvatarUrl => (User.GetAvatarUrl() ?? ImageUrl.DefaultUserAvatar(User.Discriminator)).ToString();

    public DiscordUserInfo(IUserPlatform platform, User user) {
        Platform = platform;
        User = user;
    }
    
    public override string ToString() => $"{Name} ({Key})";
}
