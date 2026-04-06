using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Commands;
using PamelloV7.Framework.Entities;
using PamelloV7.Framework.Platforms;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Attributes;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Modals.Song;

[DiscordModal("Select song source")]

[AddSelect<int>("SourceIndex", "Source")]

public partial class SongSelectSourceModal
{
    public partial class Builder
    {
        public void Build(IPamelloSong song) {
            var counter = 0;
            
            SourceIndex.Options = song.Sources.Select(source =>
                new StringMenuSelectOptionProperties(source.PK.Key, counter.ToString())
                    .WithEmoji(EmojiProperties.Custom(Clients.Emojis.FirstOrDefault(e => e.Name == source.PK.Platform)?.Id ?? 0))
                    .WithDefault(counter++ == song.SelectedSourceIndex)
            ).ToList();
        }
    }
    
    public void Submit(IPamelloSong song) {
        Command<SongSourceSelect>().Execute(song, SourceIndex);
    }
}
