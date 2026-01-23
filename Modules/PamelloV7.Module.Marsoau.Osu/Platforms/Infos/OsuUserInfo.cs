using osu.NET.Models.Users;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;

namespace PamelloV7.Module.Marsoau.Osu.Platforms.Infos;

public class OsuUserInfo : IUserInfo
{
    public IUserPlatform Platform { get; }
    
    public UserExtended User { get; }
    
    public string Key => User.Id.ToString();
    public string Name => User.Username;
    public string AvatarUrl => User.AvatarUrl;

    public OsuUserInfo(IUserPlatform platform, UserExtended user) {
        Platform = platform;
        User = user;
    }
    
    public override string ToString() => $"{Name} ({Key})";
}
