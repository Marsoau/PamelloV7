using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.User.Authorization;

[DiscordCommand("user authorization list", "View/Manage your authorizations")]
public partial class UserAuthorizationList
{
    public async Task Execute(
        [UserDescription] IPamelloUser? user = null
    ) {
        user ??= ScopeUser;
        
        await RespondAsync(() => 
            Builder<Builder>().Build(user)
        , () => [ScopeUser]);
    }
    
    public class Builder : DiscordComponentBuilder
    {
        public IMessageComponentProperties?[] Build(IPamelloUser user) {
            var container = new ComponentContainerProperties();

            var isOwner = user == ScopeUser;
            
            container.AddComponents(
                new TextDisplayProperties($"## Authorizations{(
                    isOwner ? "" : $" of {user.ToDiscordString()}"
                )}")
            );

            return [
                container
            ];
        }
    }
}
