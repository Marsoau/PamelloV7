using PamelloV7.Core.Model.Entities;
using PamelloV7.Core.Services;

namespace PamelloV7.Module.Marsoau.Base.Services;

public class UserAuthorizationService : IUserAuthorizationService
{
    private readonly Dictionary<int, IPamelloUser> _records;
    
    public UserAuthorizationService() {
        _records = [];
    }
    
    public int GetCode(IPamelloUser user) {
        int code;
        if (_records.ContainsValue(user)) {
            code = _records.First(record => record.Value == user).Key;
            return code;
        }
        
        code = Random.Shared.Next(100000, 999999);
        _records.Add(code, user);

        return code;
    }

    public IPamelloUser? GetUser(int code) {
        if (_records.Remove(code, out var user)) return user;
        
        return null;
    }
}
