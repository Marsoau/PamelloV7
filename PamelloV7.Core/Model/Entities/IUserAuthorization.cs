namespace PamelloV7.Core.Model.Entities;

public interface IUserAuthorization
{
    public IPamelloUser User { get; }
    
    public string Service { get; }
    public string Key { get; }
    
    public string UserName { get; }
    public string UserAvatarUrl { get; }
}
