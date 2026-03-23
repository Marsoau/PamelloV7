using Discord;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Difference;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.Discord.Attributes;
using PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Base;

namespace PamelloV7.Module.Marsoau.Discord.Interactions.Modals.Song;

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
    
    [ModalSubmission("song-edit-associations-modal")]
    public async Task Submit(string songQuery) {
        var song = await GetSingleRequiredAsync<IPamelloSong>(songQuery);

        var newAssociationsStr = GetInputValue("modal-input");
        var newAssociations = newAssociationsStr.Split(',').SelectMany(a => a.Split('\n'));

        var differenceResult = DifferenceResult<string>.From(song.Associations, newAssociations, null, true);
        
        await ReleaseInteractionAsync();
        
        foreach (var (at, association) in differenceResult.Added) {
            StaticLogger.Log($"+{association}");
            Command<SongAssociationsAdd>().Execute(song, association);
        }
        foreach (var (at, association) in differenceResult.Deleted) {
            StaticLogger.Log($"-{association}");
            Command<SongAssociationsRemove>().Execute(song, association);
        }
    }
}
