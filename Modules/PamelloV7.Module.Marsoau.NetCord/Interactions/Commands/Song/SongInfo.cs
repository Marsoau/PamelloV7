using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;
using PamelloV7.Module.Marsoau.NetCord.Services;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Song;

[DiscordCommand("song info", "Get info about a song")]
public partial class SongInfo
{
    public async Task Execute(
        [SongDescription] [DefaultQuery("current")] IPamelloSong song
    ) {
        await RespondAsync(() =>
            Builder<Builder>().Build(song)
        , () => [song, song.AddedBy, ..song.FavoriteBy, ..song.Playlists]);
    }

    public class Builder : DiscordComponentBuilder
    {
        public ComponentContainerProperties Build(IPamelloSong song) {
            Uri coverUrl;
            try {
                coverUrl = new Uri(song.CoverUrl);
            }
            catch {
                coverUrl = new Uri("https://cdn.discordapp.com/embed/avatars/0.png");
            }

            var clients = Services.GetRequiredService<DiscordClientService>();

            var sourcesBuilder = new StringBuilder();
            foreach (var source in song.Sources) {
                var line = $"{DiscordString.Emoji(source.PK.Platform, clients)} {DiscordString.Url(source.PK.Key, source.GetUrl())}";

                if (source == song.SelectedSource) {
                    sourcesBuilder.AppendLine(DiscordString.None($"{line} {DiscordString.Bold(DiscordString.Code("<"))}"));
                    continue;
                }

                sourcesBuilder.AppendLine(line);
            }

            var container = new ComponentContainerProperties().AddComponents(
                new ComponentSectionProperties(
                    new ComponentSectionThumbnailProperties(
                        new ComponentMediaProperties(coverUrl.ToString())
                    )
                ).AddComponents(
                    new TextDisplayProperties(
                        $"""
                         ## {song.Name}

                         -# Id: {song.Id}
                         """
                    )
                ),
                new ComponentSeparatorProperties(),
                new ActionRowProperties().AddComponents(
                    ModalButton<SongRenameModal>("Rename", ButtonStyle.Secondary, [song]),
                    Button("Change Cover", ButtonStyle.Secondary, () => { })
                        .WithDisabled(),
                    Button("Reset", ButtonStyle.Secondary, async () => {
                        await Command<SongInfoReset>().Execute(song);
                    })
                ),
                new ComponentSeparatorProperties(),
                new TextDisplayProperties(
                    $"""
                     - Added by {song.AddedBy?.ToDiscordString()}
                     - Added at {DiscordString.Time(song.AddedAt)}
                     """
                ),
                new ComponentSeparatorProperties(),
                new ComponentSectionProperties(
                    ModalButton<SongEditAssociationsModal>("Edit", ButtonStyle.Secondary, [song])
                ).AddComponents(
                    new TextDisplayProperties(
                        $"""
                         ### Associations
                         {(song.Associations.Count == 0 ? "-# _None_" : "")}
                         {string.Join("\n", song.Associations)}
                         """
                    )
                ),
                new ComponentSectionProperties(
                    Button(song.FavoriteBy.Contains(ScopeUser) ? "Remove" : "Add", ButtonStyle.Secondary, () => {
                        if (song.FavoriteBy.Contains(ScopeUser)) {
                            Command<SongFavoritesRemove>().Execute([song]);
                        }
                        else {
                            Command<SongFavoritesAdd>().Execute([song]);
                        }
                    })
                ).AddComponents(
                    new TextDisplayProperties(
                        $"""
                         ### Favorite By Users
                         {(song.FavoriteBy.Count == 0 ? "-# _None_" : "")}
                         {string.Join("\n", song.FavoriteBy.Select(user => user.ToDiscordString()))}
                         """
                    )
                ),
                new ComponentSectionProperties(
                    Button("Remove All", ButtonStyle.Secondary, () => {
                        //todo, use single SongPlaylistRemove?playlists command for this in the future

                        foreach (var playlist in song.Playlists.ToList()) {
                            Command<PlaylistSongRemove>().Execute(playlist, [song]);
                        }
                    }).WithDisabled(song.Playlists.Count == 0)
                ).AddComponents(
                    new TextDisplayProperties(
                        $"""
                         ### Included In Playlists
                         {(song.Playlists.Count == 0 ? "-# _None_" : "")}
                         {string.Join("\n", song.Playlists.Select(playlist => playlist.ToDiscordString()))}
                         """
                    )
                ),
                new ComponentSectionProperties(
                    ModalButton<SongSelectSourceModal>("Select", ButtonStyle.Secondary, [song])
                ).AddComponents(
                    new TextDisplayProperties(
                        $"""
                         ### Sources
                         {(song.Sources.Count == 0 ? "-# _None_" : "")}
                         {sourcesBuilder}
                         """
                    )
                )
            );

            if (song.Episodes.Count > 0) {
                container.AddComponents(
                    new ComponentSectionProperties(
                        Button("List Episodes", ButtonStyle.Secondary, () => { })
                            .WithDisabled()
                    ).AddComponents(
                        new TextDisplayProperties($"### Episodes: `{song.Episodes.Count}`\n")
                    )
                );
            }

            container.AddComponents(
                new ActionRowProperties().AddComponents(
                    Button("Add to queue", ButtonStyle.Primary, () => {
                        Command<PlayerQueueSongAdd>().Execute([song]);
                    })
                )
            );

            return container;
        }
    }
}
