using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Playlist;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Playlist;

[DiscordCommand("playlist info", "View/Manage a playlist")]
public partial class PlaylistInfo
{
    public async Task Execute(
        [PlaylistDescription] IPamelloPlaylist playlist
    ) {
        await RespondPageAsync(() => (playlist.Songs.Count, 10), page =>
            Builder<Builder>().Build(playlist, page, 10)
        , () => [playlist, ..playlist.Songs, playlist.Owner]);
    }

    public class Builder : DiscordComponentBuilder
    {
        public IMessageComponentProperties?[] Build(IPamelloPlaylist playlist, int page, int pageSize) {
            var container = new ComponentContainerProperties();
            
            var isOwner = playlist.Owner == ScopeUser;

            container.AddComponents(
                new TextDisplayProperties($"## {DiscordString.Code($"[{playlist.Id}]")} {playlist.Name}"),
                new ComponentSeparatorProperties(),
                new ActionRowProperties().AddComponents(
                    Button("Add To Queue", ButtonStyle.Primary, () => {
                        Command<PlayerQueuePlaylistAdd>().Execute([playlist]);
                    }),
                    ModalButton<PlaylistRenameModal>("Rename", ButtonStyle.Secondary, [playlist])
                        .WithDisabled(!isOwner && playlist.IsProtected),
                    ModalButton<PlaylistEditModal>("Edit", ButtonStyle.Secondary, [playlist])
                        .WithDisabled(!isOwner && playlist.IsProtected)
                ),
                new ComponentSeparatorProperties(),
                new TextDisplayProperties(
                    $"""
                     - Added at {DiscordString.Time(playlist.AddedAt)}
                     """
                ),
                new ComponentSeparatorProperties(),
                new ComponentSectionProperties(
                    ModalButton<PlaylistTransferOwnershipModal>("Transfer", ButtonStyle.Secondary, [playlist])
                        .WithDisabled(!isOwner)
                ).AddComponents(
                    new TextDisplayProperties($"### Owner: {playlist.Owner.ToDiscordString()}")
                ),
                new ComponentSectionProperties(
                    Button("Toggle", ButtonStyle.Secondary, () => {
                        Command<PlaylistIsProtectedToggle>().Execute(playlist);
                    }).WithDisabled(!isOwner)
                ).AddComponents(
                    new TextDisplayProperties($"### Is Protected: {DiscordString.Code(playlist.IsProtected)}")
                ),
                new ComponentSeparatorProperties()
            );
            
            var totalPages = playlist.Songs.Count / pageSize + (playlist.Songs.Count % pageSize > 0 ? 1 : 0);
            if (totalPages == 0) totalPages = 1;

            var itemsOnPage = playlist.Songs.Skip(page * pageSize).Take(pageSize).ToList();

            if (itemsOnPage.Count > 0) {
                container.AddComponents(
                    new TextDisplayProperties(GetEntriesText(itemsOnPage, page * pageSize))
                );
            }
            else {
                container.AddComponents(
                    new TextDisplayProperties("-# _Playlist is empty_")
                );
            }

            container.AddComponents(
                new ComponentSeparatorProperties(),
                new ComponentSectionProperties(
                    Button("Clear", ButtonStyle.Secondary, () => {
                        Command<PlaylistClear>().Execute(playlist);
                    }).WithDisabled(playlist.Songs.Count == 0 || (!isOwner && playlist.IsProtected))
                ).AddComponents(
                    new TextDisplayProperties($"-# Page {page + 1}/{totalPages} ({playlist.Songs.Count} songs)")
                )
            );
            
            return [
                container,
                Builder<BasicButtonsBuilder>().PageButtons(page, pageSize, playlist.Songs.Count)
            ];
        }
    }
}
