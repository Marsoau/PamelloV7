using PamelloV7.Core.Entities;
using PamelloV7.Core.Platforms.Infos;

namespace PamelloV7.Core.Platforms;

public interface IUserPlatform
{
    public string Name { get; }
    public string IconUrl { get; }
    
    public IUserInfo? GetUserInfo(string value);
    public IPamelloUser? GetUser(string value, bool createIfNotExist = false);
}
