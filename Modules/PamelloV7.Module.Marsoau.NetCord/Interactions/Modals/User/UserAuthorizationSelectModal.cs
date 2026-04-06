using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.User;

[DiscordModal("Select Authorization")]

[AddSelect<int>("AuthorizationIndex", "Authorization")]

public partial class UserAuthorizationSelectModal
{
    public partial class Builder
    {
        public void Build() {
            var counter = 0;
            
            AuthorizationIndex.Options = ScopeUser.Authorizations.Select(authorization =>
                new StringMenuSelectOptionProperties(authorization.PK.Key, counter.ToString())
                    .WithEmoji(EmojiProperties.Custom(Clients.Emojis.FirstOrDefault(e => e.Name == authorization.PK.Platform)?.Id ?? 0))
                    .WithDefault(counter++ == ScopeUser.SelectedAuthorizationIndex)
            ).ToList();
        }
    }
    
    public void Submit() {
        Command<UserAuthorizationSelect>().Execute(AuthorizationIndex);
    }
}
