using PamelloV7.Core.Model.Entities;

namespace PamelloV7.Core.Platforms;

public interface IUserPlatform
{
    public string Name { get; }
    public string IconUrl { get; }
    
    public IUserInfo? GetUserInfo(string value);
    public IPamelloUser? GetUser(string value, bool createIfNotExist = false);
}
