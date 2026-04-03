using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Base;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;


[DiscordModal("Song Rename")]
public partial class SongRenameModal
{
    [AddShortInput("Name*")]

    [AddParagraphInput("Description")]

    [AddSelect("Type")]
    [AddSelectOption("Song")]
    [AddSelectOption("Episode")]
    [AddSelectOption("Playlist")]

    [AddCheckBox("Is Public")]

    [AddCheckBoxGroup("Selections")]
    [AddCheckBoxOption("Selection 1")]
    [AddCheckBoxOption("Selection 2")]
    [AddCheckBoxOption("Selection 3")]

    [DiscordModalBuilder]
    public partial class SongRenameModalBuilder
    {
        public override ModalProperties Build() {
            Properties.AddComponents(
                new TextDisplayProperties("This is a text dispalay")
            );
            return base.Build();
        }
    }

    public async Task Submit(IPamelloSong song) {
    }
}
