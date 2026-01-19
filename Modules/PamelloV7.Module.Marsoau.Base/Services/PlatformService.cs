using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;
using PamelloV7.Core.Services;

namespace PamelloV7.Module.Marsoau.Base.Services;

public class PlatformService : IPlatformService
{
    private readonly IServiceProvider _services;
    
    private readonly List<ISongPlatform> _songPlatforms;
    private readonly List<IUserPlatform> _userPlatforms;
    
    public PlatformService(IServiceProvider services) {
        _services = services;
        
        _songPlatforms = [];
        _userPlatforms = [];
    }

    public void Load() {
        var typeResolver = _services.GetRequiredService<IAssemblyTypeResolver>();
        
        var platformsTypes = typeResolver.GetInheritorsOf(typeof(ISongPlatform), typeof(IUserPlatform));

        foreach (var platformType in platformsTypes) {
            switch (Activator.CreateInstance(platformType, _services)) {
                case ISongPlatform songPlatform:
                    _songPlatforms.Add(songPlatform);
                    break;
                case IUserPlatform userPlatform:
                    _userPlatforms.Add(userPlatform);
                    break;
            }
        }
    }
    
    public ISongPlatform? GetSongPlatform(string name) {
        return _songPlatforms.FirstOrDefault(x => x.Name == name);
    }

    public ISongInfo? GetSongInfo(string value) {
        foreach (var platform in _songPlatforms) {
            var songInfo = platform.GetSongInfo(value);
            if (songInfo is not null) return songInfo;
        }
        
        return null;
    }

    public PlatformKey? GetSongPlatformKey(string value) {
        string? key;
        foreach (var platform in _songPlatforms) {
            try {
                key = platform.ValueToKey(value);
            }
            catch {
                key = null;
            }
            
            if (key is not null) return new PlatformKey(platform.Name, key);
        }
        
        return null;
    }

    public IUserPlatform? GetUserPlatform(string name) {
        return _userPlatforms.FirstOrDefault(x => x.Name == name);
    }

    public IUserInfo? GetUserInfo(string value) {
        foreach (var platform in _userPlatforms) {
            var userInfo = platform.GetUserInfo(value);
            if (userInfo is not null) return userInfo;
        }
        
        return null;
    }
}
