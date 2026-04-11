using PamelloV7.Core.Audio;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Logging;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;

[DiscordModal("Edit episode")]

[AddShortInput("Name*", "Name")]
[AddShortInput<AudioTime>("Start*", "Start")]
[AddCheckBox("AutoSkip", "Auto Skip")]

public partial class SongEpisodeEditModal
{
    public partial class Builder
    {
        public void Build(IPamelloEpisode episode) {
            Name.Value = episode.Name;
            Start.Value = episode.Start.ToString();
            AutoSkip.Default = episode.AutoSkip;
        }
    }
    
    public void Submit(IPamelloEpisode episode) {
        if (episode.Name != Name) {
            Command<EpisodeRename>().Execute(episode, Name);
        }
        if (episode.Start.TotalSeconds != Start.TotalSeconds) {
            Command<EpisodeStartSet>().Execute(episode, Start);
        }
        if (episode.AutoSkip != AutoSkip) {
            Command<EpisodeAutoSkipSet>().Execute(episode, AutoSkip);
        }
    }
}
