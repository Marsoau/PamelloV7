using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public partial class UserAuthorizationAdd : PamelloCommand
{
    public bool Execute(string platform, string key, bool force = false) {
        if (force) {
            ScopeUser.AddAuthorizationForced(platform, key);
        }
        else {
            ScopeUser.AddAuthorization(platform, key);
        }

        return true;
    }
}
