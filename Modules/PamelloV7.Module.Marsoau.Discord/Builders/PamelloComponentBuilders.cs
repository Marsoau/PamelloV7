using Discord;
using PamelloV7.Core.Entities;
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
    public static ComponentBuilderV2 SongInfo(IPamelloSong song, IPamelloUser scopeUser) {
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
                                          Added at {new DiscordString(song.AddedAt)}

                                          -# id: {song.Id}
                                          """)
                    )
                    .WithSeparator()
                    .WithActionRow(new ActionRowBuilder()
                        .WithButton(new ButtonBuilder()
                            .WithCustomId("song-info-edit-name")
                            .WithLabel("Edit name")
                            .WithStyle(ButtonStyle.Secondary)
                        )
                        .WithButton(new ButtonBuilder()
                            .WithCustomId("song-info-edit-cover")
                            .WithLabel("Change cover")
                            .WithStyle(ButtonStyle.Secondary)
                        )
                        .WithButton(new ButtonBuilder()
                            .WithCustomId("song-info-reset")
                            .WithLabel("Reset")
                            .WithStyle(ButtonStyle.Secondary)
                        )
                    )
                    .WithSeparator()
                    .WithSection(new SectionBuilder()
                        .WithAccessory(new ButtonBuilder()
                            .WithCustomId("song-info-associations-edit")
                            .WithLabel("Edit")
                            .WithStyle(ButtonStyle.Secondary)
                        )
                        .WithTextDisplay($"""
                                          ### Associations
                                          {(song.FavoriteBy.Count == 0 ? "-# _None_" : "")}
                                          {string.Join("\n", song.Associations)}
                                          """)
                    )
                    .WithSection(new SectionBuilder()
                        .WithAccessory(new ButtonBuilder()
                            .WithCustomId("song-info-favorite-add")
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
                        )
                        .WithTextDisplay($"""
                                          ### Included In Playlists
                                          {(song.Playlists.Count == 0 ? "-# _None_" : "")}
                                          {string.Join("\n", song.Playlists.Select(playlist => playlist.ToDiscordString()))}
                                          """)
                    )
                );

            if (song.Episodes.Count > 0) {
                containerBuilder
                    .WithSection(new SectionBuilder()
                        .WithAccessory(new ButtonBuilder()
                            .WithCustomId("asd")
                            .WithStyle(ButtonStyle.Secondary)
                            .WithLabel("List episodes")
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
}
