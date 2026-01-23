using System.Text;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Entities.Base;
using PamelloV7.Core.Platforms.Infos;
using PamelloV7.Module.Marsoau.Discord.Enumerators;
using PamelloV7.Module.Marsoau.Discord.Services;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Builders;

public class PamelloComponentBuilders
{
    public static ComponentBuilderV2 Defer() {
        return new ComponentBuilderV2().WithTextDisplay("-# _loading..._");
    }
    
    public static ComponentBuilderV2 Info(string? content)
        => Info(null, content);
    public static ComponentBuilderV2 Info(string? title, string? content) {
        return new ComponentBuilderV2()
            .WithContainer(new ContainerBuilder()
                .WithTextDisplay(
                    (title?.Length > 0 ? $"### {title}\n" : "") +
                    (content?.Length > 0 ? $"{content}\n" : "")
                )
            );
    }
    public static ComponentBuilderV2 SongInfo(IPamelloSong song, IPamelloUser scopeUser, IServiceProvider services) {
            ContainerBuilder containerBuilder;
            
            var coverUrl = song.CoverUrl;

            var componentBuilder = new ComponentBuilderV2()
                .WithContainer(containerBuilder = new ContainerBuilder()
                    .WithSection(new SectionBuilder()
                        .WithAccessory(new ThumbnailBuilder()
                            .WithMedia(new UnfurledMediaItemProperties(coverUrl))
                        )
                        .WithTextDisplay($"""
                                          ## {song.Name}

                                          -# Id: {song.Id}
                                          """)
                    )
                    .WithSeparator()
                    .WithActionRow(new ActionRowBuilder()
                        .WithButton(new ButtonBuilder()
                            .WithCustomId($"song-info-edit-name:{song.Id}")
                            .WithLabel("Edit name")
                            .WithStyle(ButtonStyle.Secondary)
                        )
                        .WithButton(new ButtonBuilder()
                            .WithCustomId("song-info-edit-cover")
                            .WithLabel("Change cover")
                            .WithStyle(ButtonStyle.Secondary)
                            .WithDisabled(true)
                        )
                        .WithButton(new ButtonBuilder()
                            .WithCustomId($"song-info-reset:{song.Id}")
                            .WithLabel("Reset")
                            .WithStyle(ButtonStyle.Secondary)
                        )
                    )
                    .WithSeparator()
                    .WithTextDisplay($"""
                                      - Added by {song.AddedBy?.ToDiscordString()}
                                      - Added at {DiscordString.Time(song.AddedAt)}
                                      """)
                    .WithSeparator()
                    .WithSection(new SectionBuilder()
                        .WithAccessory(new ButtonBuilder()
                            .WithCustomId($"song-info-associations-edit:{song.Id}")
                            .WithLabel("Edit")
                            .WithStyle(ButtonStyle.Secondary)
                        )
                        .WithTextDisplay($"""
                                          ### Associations
                                          {(song.Associations.Count == 0 ? "-# _None_" : "")}
                                          {string.Join("\n", song.Associations)}
                                          """)
                    )
                    .WithSection(new SectionBuilder()
                        .WithAccessory(new ButtonBuilder()
                            .WithCustomId($"song-info-favorite:{song.Id}")
                            .WithLabel(song.FavoriteBy.Contains(scopeUser) ? "Remove" : "Add")
                            .WithStyle(ButtonStyle.Secondary)
                        )
                        .WithTextDisplay($"""
                                          ### Favorite By Users
                                          {(song.FavoriteBy.Count == 0 ? "-# _None_" : "")}
                                          {string.Join("\n", song.FavoriteBy.Select(user => user.ToDiscordString()))}
                                          """)
                    )
                    .WithSection(new SectionBuilder()
                        .WithAccessory(new ButtonBuilder()
                            .WithCustomId("song-info-clear-from-playlists")
                            .WithLabel("Remove all")
                            .WithStyle(ButtonStyle.Secondary)
                            .WithDisabled(true) //.WithDisabled(song.Playlists.Count == 0)
                        )
                        .WithTextDisplay($"""
                                          ### Included In Playlists
                                          {(song.Playlists.Count == 0 ? "-# _None_" : "")}
                                          {string.Join("\n", song.Playlists.Select(playlist => playlist.ToDiscordString()))}
                                          """)
                    )
                    .WithTextDisplay($"""
                                      ### Sources
                                      {(song.Sources.Count == 0 ? "-# _None_" : "")}
                                      {string.Join("\n", song.Sources.Select(source => source.ToDiscordString(services).Result))}
                                      """)
                );

            if (song.Episodes.Count > 0) {
                containerBuilder
                    .WithSection(new SectionBuilder()
                        .WithAccessory(new ButtonBuilder()
                            .WithCustomId("asd")
                            .WithStyle(ButtonStyle.Secondary)
                            .WithLabel("List episodes")
                            .WithDisabled(true)
                        )
                        .WithTextDisplay($"### Episodes: `{song.Episodes.Count}`\n")
                    );
            }

            containerBuilder
                .WithActionRow(new ActionRowBuilder()
                    .WithButton(new ButtonBuilder()
                        //.WithCustomId("player-queue-song-add")
                        .WithCustomId("refresh")
                        .WithLabel("Add to queue")
                        .WithStyle(ButtonStyle.Primary)
                    )
                )
                ;
            
            return componentBuilder;
    }

    public static ComponentBuilderV2 RefreshButton(ComponentBuilderV2 componentBuilder) {
        componentBuilder
            .WithActionRow(new ActionRowBuilder()
                .WithButton(new ButtonBuilder()
                    .WithCustomId("refresh")
                    .WithLabel("Refresh")
                    .WithStyle(ButtonStyle.Secondary)
                )
            );
        
        return componentBuilder;
    }

    public static ComponentBuilderV2 PageButtons(ComponentBuilderV2 pageBuilder, bool displayPrev, bool displayNext) {
        var actionRow = new ActionRowBuilder();

        if (displayPrev) {
            actionRow.WithButton(new ButtonBuilder()
                .WithCustomId("page-prev")
                .WithLabel("Prev")
                .WithStyle(ButtonStyle.Secondary)
            );
        }
        if (displayNext) {
            actionRow.WithButton(new ButtonBuilder()
                .WithCustomId("page-next")
                .WithLabel("Next")
                .WithStyle(ButtonStyle.Secondary)
            );
        }

        if (!displayPrev && !displayNext) return pageBuilder;
        
        pageBuilder.WithActionRow(actionRow);

        return pageBuilder;
    }

    public static ComponentBuilderV2 FavoriteList(IPamelloUser user, ESongOrPlaylist songOrPlaylist, IPamelloUser scopeUser, int page, int pageSize) {
        IReadOnlyList<IPamelloEntity> items = songOrPlaylist switch {
            ESongOrPlaylist.Song => user.FavoriteSongs,
            ESongOrPlaylist.Playlist => user.FavoritePlaylists,
            _ => throw new ArgumentOutOfRangeException(nameof(songOrPlaylist))
        };
        
        var title = user == scopeUser ? $"Favorite {songOrPlaylist.ToShortString()}s" : $"Favorite {songOrPlaylist.ToShortString()}s of {user.ToDiscordString()}";

        var totalPages = items.Count / pageSize + (items.Count % pageSize > 0 ? 1 : 0);
        if (totalPages == 0) totalPages = 1;
        
        var itemsOnPage = items.Skip(page * pageSize).Take(pageSize).ToList();
        
        var counter = page * pageSize + 1;
        var content = items.Count == 0 ? $"-# _No {songOrPlaylist.ToShortString()}s_" :
            string.Join("\n", itemsOnPage.Select(item => $"`{counter++}` : {songOrPlaylist switch {
                ESongOrPlaylist.Song => ((IPamelloSong)item).ToDiscordString(),
                ESongOrPlaylist.Playlist => ((IPamelloPlaylist)item).ToDiscordString(),
            }}"));
        
        var containerBuilder = new ContainerBuilder();

        containerBuilder
            .WithTextDisplay($"## {title}");

        if (user == scopeUser) {
            containerBuilder.WithSeparator();
            containerBuilder.WithActionRow(new ActionRowBuilder()
                .WithButton(new ButtonBuilder()
                    .WithCustomId($"favorite-{songOrPlaylist.ToShortString()}s-edit:{user.Id}")
                    .WithLabel("Edit")
                    .WithStyle(ButtonStyle.Secondary)
                )
                .WithButton(new ButtonBuilder()
                    .WithCustomId($"favorite-{songOrPlaylist.ToShortString()}s-clear:{user.Id}")
                    .WithLabel("Clear")
                    .WithDisabled(items.Count == 0)
                    .WithStyle(ButtonStyle.Secondary)
                )
            );
        }

        containerBuilder
            .WithSeparator()
            .WithTextDisplay(content)
            .WithSeparator();

        if (items.Count > 0) {
            containerBuilder
                .WithSection(new SectionBuilder()
                    .WithAccessory(new ButtonBuilder()
                        .WithCustomId("refresh")
                        //.WithCustomId($"favorite-list-add:{user.Id}")
                        .WithLabel("Add all to queue")
                        .WithStyle(ButtonStyle.Primary)
                    )
                    .WithTextDisplay($"-# Page {page + 1}/{totalPages} ({items.Count} songs)")
                );
        }
        else {
            containerBuilder
                .WithTextDisplay($"-# Page {page + 1}/{totalPages} ({items.Count} songs)");
        }
        
        var componentBuilder = new ComponentBuilderV2().WithContainer(containerBuilder);

        return PageButtons(componentBuilder, page > 0, page < totalPages - 1);
    }

    public static async Task<ComponentBuilderV2> UserInfo(IPamelloUser user, IPamelloUser scopeUser, IServiceProvider services) {
        var containerBuilder = new ContainerBuilder();

        var clients = services.GetRequiredService<DiscordClientService>();
        
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

        if (user == scopeUser) {
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
