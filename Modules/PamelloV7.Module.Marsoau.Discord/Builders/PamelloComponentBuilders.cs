using Discord;
using PamelloV7.Core.Entities;
using PamelloV7.Core.Platforms.Infos;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Builders;

public class PamelloComponentBuilders
{
    public static ComponentBuilderV2 Defer() {
        return new ComponentBuilderV2().WithTextDisplay("-# loading...");
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

            var componentBuilder = new ComponentBuilderV2()
                .WithContainer(containerBuilder = new ContainerBuilder()
                    .WithSection(new SectionBuilder()
                        .WithAccessory(new ThumbnailBuilder()
                            .WithMedia(new UnfurledMediaItemProperties(song.CoverUrl))
                        )
                        .WithTextDisplay($"""
                                          ## {song.Name}
                                          Added by {song.AddedBy?.ToDiscordString()}
                                          Added at {DiscordString.Time(song.AddedAt)}

                                          -# id: {song.Id}
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
                        .WithCustomId("song-info-queue-add")
                        .WithLabel("Add to queue")
                        .WithStyle(ButtonStyle.Primary)
                    )
                )
                ;
            
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

    public static ComponentBuilderV2 FavoriteList(IPamelloUser user, IPamelloUser scopeUser, int page, int pageSize) {
        var title = user == scopeUser ? "Favorite songs" : $"Favorite songs of {user.ToDiscordString()}";
        var totalPages = user.FavoriteSongs.Count / pageSize + (user.FavoriteSongs.Count % pageSize > 0 ? 1 : 0);
        if (totalPages == 0) totalPages = 1;
        
        var songsOnPage = user.FavoriteSongs.Skip(page * pageSize).Take(pageSize).ToList();
        
        var counter = page * pageSize + 1;
        var content = user.FavoriteSongs.Count == 0 ? "-# _No songs_" :
            string.Join("\n", songsOnPage.Select(song => $"`{counter++}` : {song.ToDiscordString()}"));
        
        var containerBuilder = new ContainerBuilder();

        containerBuilder
            .WithTextDisplay($"## {title}");

        if (user == scopeUser) {
            containerBuilder.WithSeparator();
            containerBuilder.WithActionRow(new ActionRowBuilder()
                .WithButton(new ButtonBuilder()
                    .WithCustomId($"favorite-list-edit:{user.Id}")
                    .WithLabel("Edit")
                    .WithStyle(ButtonStyle.Secondary)
                )
                .WithButton(new ButtonBuilder()
                    .WithCustomId($"favorite-list-clear:{user.Id}")
                    .WithLabel("Clear")
                    .WithStyle(ButtonStyle.Secondary)
                )
            );
        }

        containerBuilder
            .WithSeparator()
            .WithTextDisplay(content)
            .WithSeparator();

        if (user.FavoriteSongs.Count > 0) {
            containerBuilder
                .WithSection(new SectionBuilder()
                    .WithAccessory(new ButtonBuilder()
                        .WithCustomId($"favorite-list-add:{user.Id}")
                        .WithLabel("Add all to queue")
                        .WithStyle(ButtonStyle.Primary)
                    )
                    .WithTextDisplay($"-# Page {page + 1}/{totalPages} ({user.FavoriteSongs.Count} songs)")
                );
        }
        else {
            containerBuilder
                .WithTextDisplay($"-# Page {page + 1}/{totalPages} ({user.FavoriteSongs.Count} songs)");
        }
        
        var componentBuilder = new ComponentBuilderV2().WithContainer(containerBuilder);
        
        return PageButtons(componentBuilder, page > 0, page < totalPages - 1);
    }
}
