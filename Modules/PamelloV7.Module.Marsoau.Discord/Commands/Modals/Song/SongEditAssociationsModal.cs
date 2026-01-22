using Discord;
using PamelloV7.Core.Commands;
using PamelloV7.Core.Difference;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Commands.Modals.Base;

namespace PamelloV7.Module.Marsoau.Discord.Commands.Modals.Song;

[Modal("song-edit-associations-modal")]
public class SongEditAssociationsModal : DiscordModal
{
    public static Modal Build(IPamelloSong song) {
        var modalBuilder = new ModalBuilder()
            .WithTitle("Edit song associations")
            .WithCustomId($"song-edit-associations-modal:{song.Id}")
            .AddComponents(new ModalComponentBuilder()
                .WithTextInput("New associations", new TextInputBuilder()
                    .WithCustomId("modal-input")
                    .WithValue(string.Join("\n", song.Associations))
                    .WithStyle(TextInputStyle.Paragraph)
                    .WithRequired(false)
                , "Associations separated by commas or newlines")
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
        var input = components.FirstOrDefault(component => component.CustomId == "modal-input");
        if (input is null) {
            Console.WriteLine("NO INPUT");
            await EndInteraction();
            return;
        }

        var newAssociationsStr = input.Value;
        var newAssociations = newAssociationsStr.Split(',').SelectMany(a => a.Split('\n'));

        var differenceResult = DifferenceResult<string>.From(song.Associations, newAssociations, null, true);
        
        foreach (var (at, association) in differenceResult.Added) {
            Console.WriteLine($"+{association}");
            Command<SongAssociationsAdd>().Execute(song, association);
        }
        foreach (var (at, association) in differenceResult.Deleted) {
            Console.WriteLine($"-{association}");
            Command<SongAssociationsRemove>().Execute(song, association);
        }
        
        await EndInteraction();
    }
}
