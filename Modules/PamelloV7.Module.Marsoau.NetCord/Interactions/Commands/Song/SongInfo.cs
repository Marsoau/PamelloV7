using NetCord;
using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Attributes;
using PamelloV7.Module.Marsoau.NetCord.Builders;
using PamelloV7.Module.Marsoau.NetCord.Descriptions;
using PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Base;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Interactions.Commands.Song;

[DiscordCommand("song info", "Get info about a song")]
public class SongInfo : DiscordCommand
{
    public async Task Execute(
        [SongDescription] [DefaultQuery("current")] IPamelloSong? song
    ) {
        var counter = 0;
        
        await RespondAsync(() => [
            Builder<BasicComponentsBuilder>().Info(null, $"{Interaction.Id}\nContent {counter}"),
            Builder<BasicButtonsBuilder>().RefreshButtonRow(),
            new ActionRowProperties().AddComponents(
                Button("Test", ButtonStyle.Secondary, () => {
                    counter++;
                })
            )
        ]);
    }
}
