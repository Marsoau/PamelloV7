using PamelloV7.Framework.Commands;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.User;

[DiscordModal("Add Authorization")]

[AddShortInput("Platform*", "Platform")]
[AddShortInput("Key*", "Key")]

[AddCheckBox("Force", "Force")]

public partial class UserAuthorizationAddModal
{
    public void Submit() {
        Command<UserAuthorizationAdd>().Execute(Platform, Key, Force);
    }
}
