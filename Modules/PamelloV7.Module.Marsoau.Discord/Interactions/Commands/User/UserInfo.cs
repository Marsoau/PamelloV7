using Discord.Interactions;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;
using PamelloV7.Module.Marsoau.Discord.Interactions.Modals.User;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.User;

[UserGroup]
public class UserCommand : DiscordCommand
{
    [SlashCommand("info", "Get user info")]
    public async Task Info(
        [Summary("user", "User query")] string userQuery = "current"
    ) {
        var user = await GetSingleRequiredAsync<IPamelloUser>(userQuery);

        await RespondUpdatableAsync(() =>
            Builder<UserInfoBuilder>().Component(user).Result.Build()
        , user);
    }
}

public class UserInteractions : DiscordCommand
{
    [ComponentInteraction("user-authorization-select:*")]
    public async Task SelectButton(string userQuery) {
        var user = await GetSingleRequiredAsync<IPamelloUser>(userQuery);
        
        await RespondWithModalAsync(UserAuthorizationSelectModal.Build(user, Services));
    }
}
