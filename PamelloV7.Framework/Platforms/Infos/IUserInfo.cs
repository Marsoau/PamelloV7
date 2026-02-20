namespace PamelloV7.Framework.Platforms.Infos;

public interface IUserInfo
{
    public IUserPlatform Platform { get; }
    
    public string Key { get; }
    public string Name { get; }
    public string AvatarUrl { get; }
}
