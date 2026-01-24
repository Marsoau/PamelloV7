using Discord;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Song;

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

    [ModalSubmission("song-rename-modal")]
    public async Task Submit(string songQuery) {
        var song = await GetSingleRequiredAsync<IPamelloSong>(songQuery);
        var newName = GetInputValue("song-rename-modal-input");

        Command<SongRename>().Execute(song, newName);
        
        await ReleaseInteractionAsync();
    }
}
