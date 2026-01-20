using PamelloV7.Core.Entities;
using PamelloV7.Core.Services.Base;

namespace PamelloV7.Core.Services;

public interface ICodeAuthorizationService : IPamelloService
{
    public int GetCode(IPamelloUser user);
    public IPamelloUser? GetUser(int code);
}
