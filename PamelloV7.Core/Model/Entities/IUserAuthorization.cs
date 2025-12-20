namespace PamelloV7.Core.Model.Entities;

public interface IUserAuthorization
{
    public IPamelloUser User { get; }
    
    public string Platform { get; }
    public string Key { get; }
    
    public string UserName { get; }
    public string UserAvatar { get; }
}
