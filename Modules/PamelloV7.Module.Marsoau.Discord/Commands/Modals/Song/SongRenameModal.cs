using Discord;
using Discord.WebSocket;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Commands.Modals.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Modals.Song;

[Modal("song-rename-modal")]
public class SongRenameModal : DiscordModal
{
    public static Modal Build(IPamelloSong song) {
        var modalBuilder = new ModalBuilder()
            .WithTitle("Edit song name")
            .WithCustomId($"song-rename-modal:{song.Id}")
            .AddComponents(new ModalComponentBuilder()
                .WithTextInput("New name", new TextInputBuilder()
                    .WithCustomId("song-rename-modal-input")
                    .WithRequired(true)
                    .WithValue(song.Name)
                )
            );
        
        return modalBuilder.Build();
    }

    public override async Task Submit(string songQuery) {
        var song = _peql.GetSingleRequired<IPamelloSong>(songQuery, User);
        var newName = GetInputValue("song-rename-modal-input");

        Command<SongRename>().Execute(song, newName);
        
        await ReleaseInteractionAsync();
    }
}
