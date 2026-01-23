using PamelloV7.Core.Entities;
using PamelloV7.Core.Platforms.Infos;

namespace PamelloV7.Core.Platforms;

public interface IUserPlatform
{
    public string Name { get; }
    
    public string ValueToKey(string value);
    public IUserInfo? GetUserInfo(string key);
}
