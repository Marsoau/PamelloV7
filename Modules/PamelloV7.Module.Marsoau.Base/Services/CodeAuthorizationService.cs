using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Framework.Services;

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

        _ = Task.Run(async () => {
            await Task.Delay(TimeSpan.FromMinutes(10));

            if (_records.Remove(code)) {
                Output.Write($"Removed outdated code: {code} for {user}");
            }
        });

        return code;
    }

    public IPamelloUser? GetUser(int code) {
        if (_records.Remove(code, out var user)) return user;
        
        return null;
    }
}
