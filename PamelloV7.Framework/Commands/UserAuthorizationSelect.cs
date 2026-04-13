using PamelloV7.Framework.Attributes;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Platforms;
using PamelloV7.Framework.Commands.Base;

namespace PamelloV7.Framework.Commands;

[PamelloCommand]
public partial class UserAuthorizationSelect
{
    public void Execute(int index) {
        ScopeUser.SelectAuthorization(index);
    }
}
