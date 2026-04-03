using NetCord.Rest;
using PamelloV7.Framework.Entities;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;
using PamelloV7.Module.Marsoau.NetCord.Strings;

namespace PamelloV7.Module.Marsoau.NetCord.Builders;

public class AddedSongsBuilder : DiscordComponentBuilder
{
    public ComponentContainerProperties GetForOne(IPamelloSong song) {
        return new ComponentContainerProperties().AddComponents(
            new ComponentSectionProperties(
                new ComponentSectionThumbnailProperties(song.CoverUrl)
            ).AddComponents(
                new TextDisplayProperties(
                    $"""
                     ## Song Added
                     {song.ToDiscordString()}
                     """
                )
            )
        );
    }
}
