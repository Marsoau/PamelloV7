using Discord;
using PamelloV7.Core.Entities;
using PamelloV7.Module.Marsoau.Discord.Builders.Base;
using PamelloV7.Module.Marsoau.Discord.Strings;

namespace PamelloV7.Module.Marsoau.Discord.Builders;

public class AddedSongsBuilder : PamelloDiscordComponentBuilder
{
    public ComponentBuilderV2 GetForOne(IPamelloSong song) {
        return new ComponentBuilderV2()
            .WithContainer(new ContainerBuilder()
                .WithSection(new SectionBuilder()
                    .WithAccessory(new ThumbnailBuilder()
                        .WithMedia(new UnfurledMediaItemProperties(song.CoverUrl))
                    )
                    .WithTextDisplay(
                        $"""
                         ## Song Added
                         {song.ToDiscordString()}
                         """
                    )
                )
                //.WithSeparator()
            );
    }
}
