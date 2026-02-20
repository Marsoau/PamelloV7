using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public class UserAuthorizationSelect : PamelloCommand
{
    public bool Execute(int index) {
        if (index < 0 || index >= ScopeUser.Authorizations.Count) return false;

        ScopeUser.SelectAuthorization(index);

        return true;
    }
}
