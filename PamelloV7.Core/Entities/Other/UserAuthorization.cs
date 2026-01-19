using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Platforms;
using PamelloV7.Core.Platforms.Infos;
using PamelloV7.Core.Services;

namespace PamelloV7.Core.Entities.Other;

public class UserAuthorization
{
    private readonly IPlatformService _platfroms;
    
    public IPamelloUser User { get; }
    
    private IUserInfo? _info;
    public IUserInfo? Info {
        get {
            if (_info is not null) return _info;
            return UpdateInfo();
        }
    }
    
    public PlatformKey PK { get; }
    
    public UserAuthorization(IServiceProvider services, IPamelloUser user, PlatformKey pk) {
        _platfroms = services.GetRequiredService<IPlatformService>();
        
        User = user;
        PK = pk;
    }
    
    public IUserInfo? UpdateInfo() {
        var platform = _platfroms.GetUserPlatform(PK.Platform);
        if (platform is null) return _info = null;
        
        var info = platform.GetUserInfo(PK.Key);
        return _info = info;
    }
}
