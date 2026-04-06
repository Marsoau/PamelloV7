using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.User;
using PamelloV7.Module.Marsoau.NetCord.Services;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.User;

[DiscordCommand("user info", "Get info about a user")]
public partial class UserInfo
{
    public async Task Execute(
        IPamelloUser? user = null
    ) {
        user ??= ScopeUser;
        
        await RespondAsync(() =>
            Builder<Builder>().Build(user)
        , () => [user]);
    }

    public class Builder : DiscordComponentBuilder
    {
        public IMessageComponentProperties? Build(IPamelloUser user) {
            var clients = Services.GetRequiredService<DiscordClientService>();

            var authorizationsBuilder = new StringBuilder();
            foreach (var authorization in user.Authorizations) {
                var line = $"{DiscordString.Emoji(authorization.PK.Platform, clients)} {DiscordString.Code(authorization.PK.Key)}";

                if (authorization == user.SelectedAuthorization) {
                    authorizationsBuilder.AppendLine(DiscordString.None($"{line} {DiscordString.Bold(DiscordString.Code("<"))}"));
                    continue;
                }

                authorizationsBuilder.AppendLine(line);
            }

            var avatarUrl = user.SelectedAuthorization?.Info?.AvatarUrl ?? "https://cdn.discordapp.com/embed/avatars/0.png";

            var container = new ComponentContainerProperties().AddComponents(
                new ComponentSectionProperties(
                    new ComponentSectionThumbnailProperties(
                        new ComponentMediaProperties(avatarUrl)
                    )
                ).AddComponents(
                    new TextDisplayProperties(
                        $"""
                         ## {user.Name}

                         -# Id: {user.Id}
                         """
                    )
                ),
                new ComponentSeparatorProperties(),
                new TextDisplayProperties(
                    $"""
                     - Joined at {DiscordString.Time(user.JoinedAt)}
                     """
                ),
                new ComponentSeparatorProperties(),
                new ComponentSectionProperties(
                    Button("List", ButtonStyle.Secondary, () => { })
                        .WithDisabled()
                ).AddComponents(
                    new TextDisplayProperties($"### Favorite Songs: `{user.FavoriteSongs.Count}`\n")
                ),
                new ComponentSectionProperties(
                    Button("List", ButtonStyle.Secondary, () => { })
                        .WithDisabled()
                ).AddComponents(
                    new TextDisplayProperties($"### Favorite Playlists: `{user.FavoritePlaylists.Count}`\n")
                ),
                new ComponentSeparatorProperties()
            );

            if (user == ScopeUser) {
                container.AddComponents(
                    new ComponentSectionProperties(
                        ModalButton<UserAuthorizationSelectModal>("Select", ButtonStyle.Secondary)
                    ).AddComponents(
                        new TextDisplayProperties(
                            $"""
                             ### Authorizations
                             {(user.Authorizations.Count == 0 ? "-# _None_" : "")}
                             {authorizationsBuilder}
                             """
                        )
                    )
                );
            }
            else {
                container.AddComponents(
                    new TextDisplayProperties(
                        $"""
                         ### Authorizations
                         {(user.Authorizations.Count == 0 ? "-# _None_" : "")}
                         {authorizationsBuilder}
                         """
                    )
                );
            }

            return container;
            //return [ container, Builder<BasicButtonsBuilder>().RefreshButtonRow() ];
        }
    }
}
