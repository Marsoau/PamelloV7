using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Buttons.Other;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Playlist.Favorite;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Song.Favorite;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.User.Authorization;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.User;
using PamelloV7.Module.Marsoau.NetCord.Services;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.User;

[DiscordCommand("user info", "Get info about a user")]
public partial class UserInfo
{
    public async Task Execute(
        [UserDescription] IPamelloUser? user = null
    ) {
        user ??= ScopeUser;
        
        var switcher = new CommandSwitcher(this, Services);
        
        switcher.Add<SongFavoriteList>("songs", favoriteSongs => favoriteSongs.Execute(user));
        switcher.Add<PlaylistFavoriteList>("playlists", favoritePlaylists => favoritePlaylists.Execute(user));
        switcher.Add<UserAuthorizationList>("authorizations", authorizations => authorizations.Execute(user), true);
        
        await RespondAsync(() => {
            var favoriteSongsButton = Button(switcher.StateOf("songs") ? "Hide" : "Show", ButtonStyle.Secondary, async () => {
                await switcher.Toggle("songs");
            });
            var favoritePlaylistsButton = Button(switcher.StateOf("playlists") ? "Hide" : "Show", ButtonStyle.Secondary, async () => {
                await switcher.Toggle("playlists");
            });
            var authorizationsButton = Button(switcher.StateOf("authorizations") ? "Hide" : "Show", ButtonStyle.Secondary, async () => {
                await switcher.Toggle("authorizations");
            });
            
            return Builder<Builder>().Build(user, favoriteSongsButton, favoritePlaylistsButton, authorizationsButton);
        }, () => [user]);
    }

    public class Builder : DiscordComponentBuilder
    {
        public IMessageComponentProperties? Build(
            IPamelloUser user,
            ButtonProperties favoriteSongsButton,
            ButtonProperties favoritePlaylistsButton,
            ButtonProperties authorizationsButton
        ) {
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
                    favoriteSongsButton
                ).AddComponents(
                    new TextDisplayProperties($"### Favorite Songs: `{user.FavoriteSongs.Count}`\n")
                ),
                new ComponentSectionProperties(
                    favoritePlaylistsButton
                ).AddComponents(
                    new TextDisplayProperties($"### Favorite Playlists: `{user.FavoritePlaylists.Count}`\n")
                ),
                new ComponentSeparatorProperties()
            );

            var authorizationsTextDisplay = new TextDisplayProperties(
                $"""
                 ### Authorizations
                 {(user.Authorizations.Count == 0 ? "-# _None_" : "")}
                 {authorizationsBuilder}
                 """
            );

            if (user == ScopeUser) {
                container.AddComponents(
                    new ComponentSectionProperties(
                        authorizationsButton
                    ).AddComponents(authorizationsTextDisplay)
                );
            }
            else {
                container.AddComponents(authorizationsTextDisplay);
            }

            return container;
            //return [ container, Builder<BasicButtonsBuilder>().RefreshButtonRow() ];
        }
    }
}
