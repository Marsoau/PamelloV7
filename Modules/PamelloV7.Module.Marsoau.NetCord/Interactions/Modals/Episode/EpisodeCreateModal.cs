using PamelloV7.Core.Audio;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Episode;

[DiscordModal("Edit episode")]

[AddShortInput("Name*", "Name")]
[AddShortInput<AudioTime>("Start*", "Start")]
[AddCheckBox("AutoSkip", "Auto Skip")]

public partial class EpisodeCreateModal
{
    public void Submit(IPamelloSong song) {
        Command<EpisodeCreate>().Execute(song, Start, Name, AutoSkip);
    }
}
