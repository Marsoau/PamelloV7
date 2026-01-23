using PamelloV7.Core.Commands.Base;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Platforms;

namespace PamelloV7.Core.Commands;

public class UserAuthorizationSelect : PamelloCommand
{
    public bool Execute(int index) {
        if (index < 0 || index >= ScopeUser.Authorizations.Count) return false;

        ScopeUser.SelectedAuthorizationIndex = index;

        return true;
    }
}
