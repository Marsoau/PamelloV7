using Discord.Interactions;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Groups;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Commands.Playlist;

[PlaylistGroup]
public class PlaylistRenameCommand : DiscordCommand
{
    [SlashCommand("rename", "Rename a playlist")]
    public async Task Rename(
        [Summary("name", "New name for the playlist")] string newName,
        [Summary("playlist", "Single playlist to rename")] string playlistQuery
    ) {
        var playlist = await GetSingleAsync<IPamelloPlaylist>(playlistQuery);
        if (playlist is null) {
            await RespondAsync("Nema tokogo");
            return;
        }
        
        Command<PlaylistRename>().Execute(playlist, newName);

        await RespondUpdatableAsync(() =>
            Builder<ButtonsBuilder>().RefreshButton(
                Builder<BasicComponentsBuilder>().Info("Playlist Renamed", playlist.ToDiscordString())
            ).Build()
        , playlist);
    }
}
