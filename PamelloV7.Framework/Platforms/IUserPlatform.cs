using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Platforms.Infos;

namespace PamelloV7.Framework.Platforms;

public interface IUserPlatform
{
    public string Name { get; }
    
    public string ValueToKey(string value);
    public Task<IUserInfo?> GetUserInfo(string key);
    
    public void Startup() { }
    public void Shutdown() { }
}
