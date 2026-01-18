namespace PamelloV7.Core.Platforms;

public interface IUserInfo
{
    public IUserPlatform Platform { get; }
    
    public string Key { get; }
    public string Name { get; }
    public string AvatarUrl { get; }
}
