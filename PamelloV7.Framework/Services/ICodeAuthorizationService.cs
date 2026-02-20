using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Services.Base;

namespace PamelloV7.Framework.Services;

public interface ICodeAuthorizationService : IPamelloService
{
    public int GetCode(IPamelloUser user);
    public IPamelloUser? GetUser(int code);
}
