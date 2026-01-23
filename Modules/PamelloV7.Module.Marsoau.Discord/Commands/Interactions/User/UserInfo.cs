using Discord.Interactions;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Commands.Modals.User;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Interactions.User;

public partial class User
{
    [SlashCommand("info", "Get user info")]
    public async Task Info(
        [Summary("user", "User query")] string userQuery = "current"
    ) {
        var user = _peql.GetSingle<IPamelloUser>(userQuery, Context.User);
        if (user is null) {
            await RespondAsync("Nema tokogo");
            return;
        }

        await RespondUpdatableAsync(message => {
            message.Components = PamelloComponentBuilders.UserInfo(user, Context.User, Services).Result.Build();
        }, user);
    }
}

public partial class UserInteractions
{
    [ComponentInteraction("user-authorization-select:*")]
    public async Task SelectButton(string userQuery) {
        var user = _peql.GetSingle<IPamelloUser>(userQuery, Context.User);
        if (user is null || user != Context.User) {
            await ReleaseInteractionAsync();
            return;
        }
        
        await RespondWithModalAsync(UserAuthorizationSelectModal.Build(user, Services));
    }
}