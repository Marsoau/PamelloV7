using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Platforms.Infos;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Services;

public interface IPlatformService : IPamelloService
{
    public ISongPlatform? GetSongPlatform(string name);
    public Task<ISongInfo?> GetSongInfoAsync(string key);
    public PlatformKey? GetSongPlatformKey(string value);
    
    public IUserPlatform? GetUserPlatform(string name);
    public Task<IUserInfo?> GetUserInfoAsync(string value);
}
