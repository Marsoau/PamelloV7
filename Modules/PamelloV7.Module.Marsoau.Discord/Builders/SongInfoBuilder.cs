using Discord;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Builders;

public class SongInfoBuilder : PamelloComponentBuilder
{
    public ComponentBuilderV2 Component(IPamelloSong song) {
            ContainerBuilder containerBuilder;

            Uri coverUrl;
            try {
                coverUrl = new Uri(song.CoverUrl);
            }
            catch {
                coverUrl = new Uri("https://cdn.discordapp.com/embed/avatars/0.png");
            }

            var componentBuilder = new ComponentBuilderV2()
                .WithContainer(containerBuilder = new ContainerBuilder()
                    .WithSection(new SectionBuilder()
                        .WithAccessory(new ThumbnailBuilder()
                            .WithMedia(new UnfurledMediaItemProperties(coverUrl.ToString()))
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
                            .WithLabel(song.FavoriteBy.Contains(ScopeUser) ? "Remove" : "Add")
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
                                      {string.Join("\n", song.Sources.Select(source => source.ToDiscordString(Services).Result))}
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
}
