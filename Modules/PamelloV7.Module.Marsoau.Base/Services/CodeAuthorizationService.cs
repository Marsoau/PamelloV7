using PamelloV7.Core.Entities;
using PamelloV7.Core.Services;

namespace PamelloV7.Module.Marsoau.Base.Services;

public class CodeAuthorizationService : ICodeAuthorizationService
{
    private readonly Dictionary<int, IPamelloUser> _records;
    
    public CodeAuthorizationService() {
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
