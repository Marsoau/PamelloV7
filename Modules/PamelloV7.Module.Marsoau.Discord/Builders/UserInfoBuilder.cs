using System.Text;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders.Base;
using PamelloV7.Module.Marsoau.Discord.Services;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Builders;

public class UserInfoBuilder : PamelloComponentBuilder
{
    public async Task<ComponentBuilderV2> Component(IPamelloUser user) {
        var containerBuilder = new ContainerBuilder();

        var clients = Services.GetRequiredService<DiscordClientService>();
        
        var authorizationsBuilder = new StringBuilder();
        foreach (var authorization in user.Authorizations) {
            var line = "";
            var emote = await clients.GetEmote(authorization.PK.Platform);
            if (emote is null) {
                line = $"{authorization.PK.Platform}: {DiscordString.Code(authorization.PK.Key)}";
                continue;
            }
            
            line = $"{DiscordString.Emote(emote)} {DiscordString.Code(authorization.PK.Key)}";

            if (authorization == user.SelectedAuthorization) {
                authorizationsBuilder.AppendLine(DiscordString.None($"{line} {DiscordString.Bold(DiscordString.Code("<"))}"));
                continue;
            }

            authorizationsBuilder.AppendLine(line);
        }
        
        var avatarUrl = user.SelectedAuthorization?.Info?.AvatarUrl ?? "https://cdn.discordapp.com/embed/avatars/0.png";

        containerBuilder
            .WithSection(new SectionBuilder()
                .WithAccessory(new ThumbnailBuilder()
                    .WithMedia(new UnfurledMediaItemProperties(avatarUrl))
                )
                .WithTextDisplay($"""
                                  ## {user.Name}
                                  
                                  -# Id: {user.Id}
                                  """)
            )
            .WithSeparator()
            .WithTextDisplay($"""
                              - Joined at {DiscordString.Time(user.JoinedAt)}
                              - Activity points: {DiscordString.Code(Random.Shared.Next(0, 100).ToString())}
                              """)
            .WithSeparator()
            .WithSection(new SectionBuilder()
                .WithAccessory(new ButtonBuilder()
                    .WithCustomId("asd")
                    .WithStyle(ButtonStyle.Secondary)
                    .WithLabel("List")
                    .WithDisabled(true)
                )
                .WithTextDisplay($"### Favorite Songs: `{user.FavoriteSongs.Count}`\n")
            )
            .WithSection(new SectionBuilder()
                .WithAccessory(new ButtonBuilder()
                    .WithCustomId("asda")
                    .WithStyle(ButtonStyle.Secondary)
                    .WithLabel("List")
                    .WithDisabled(true)
                )
                .WithTextDisplay($"### Favorite Playlists: `{user.FavoritePlaylists.Count}`\n")
            )
            .WithSeparator()
            ;

        if (user == ScopeUser) {
            containerBuilder
                .WithSection(new SectionBuilder()
                    .WithAccessory(new ButtonBuilder()
                        .WithCustomId($"user-authorization-select:{user.Id}")
                        .WithLabel("Select")
                        .WithStyle(ButtonStyle.Secondary)
                    )
                    .WithTextDisplay($"""
                                      ### Authorizations
                                      {(user.Authorizations.Count == 0 ? "-# _None_" : "")}
                                      {authorizationsBuilder}
                                      """)
                );
        }
        else {
            containerBuilder
                .WithTextDisplay($"""
                                  ### Authorizations
                                  {(user.Authorizations.Count == 0 ? "-# _None_" : "")}
                                  {authorizationsBuilder}
                                  """);
        }
        
        var componentBuilder = new ComponentBuilderV2().WithContainer(containerBuilder);
        
        return componentBuilder;
    }
}
