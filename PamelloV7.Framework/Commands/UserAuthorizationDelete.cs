using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

public partial class UserAuthorizationDelete : PamelloCommand
{
    public void Execute(int index) {
        ScopeUser.DeleteAuthorization(index);
    }
}
