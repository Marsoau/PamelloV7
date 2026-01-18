using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Services;

public interface IPlatformService : IPamelloService
{
    public ISongPlatform? GetSongPlatform(string name);
    public ISongInfo? GetSongInfo(string value);
    public IPamelloSong? GetSong(string value);
    public PlatformKey? GetSongPlatformKey(string value);
    
    public IUserPlatform? GetUserPlatform(string name);
    public IUserInfo? GetUserInfo(string value);
}
