using Discord;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Builders.Components;

public static class SkippedSongComponent
{
    public static ComponentBuilderV2 Get(IPamelloSong? oldSong, IPamelloSong? newSong) {
        var container = new ContainerBuilder()
            .WithTextDisplay(oldSong is not null ? $"{DiscordString.Bold("Skipped")} {oldSong.ToDiscordString()}" : "Nothing was skipped");

        if (newSong is not null) {
            container
                .WithSeparator()
                .WithTextDisplay($"{DiscordString.Bold("Playing")} {newSong.ToDiscordString()}");
        }
        
        return new ComponentBuilderV2()
            .WithContainer(container);
    }
}
