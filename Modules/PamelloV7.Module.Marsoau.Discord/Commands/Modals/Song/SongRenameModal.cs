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
        var song = _peql.GetSingle<IPamelloSong>(songQuery, User);
        if (song is null) {
            Console.WriteLine("NO SONG");
            await EndInteraction();
            return;
        }
        
        var components = Modal.Data.Components.ToArray();
        var input = components.FirstOrDefault(component => component.CustomId == "song-rename-modal-input");
        if (input is null) {
            Console.WriteLine("NO INPUT");
            await EndInteraction();
            return;
        }

        var newName = input.Value;

        Command<SongRename>().Execute(song, newName);
        
        Console.WriteLine("RENAMED");
        await EndInteraction();
    }
}
