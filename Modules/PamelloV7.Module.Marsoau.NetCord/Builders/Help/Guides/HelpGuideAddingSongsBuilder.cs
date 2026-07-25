using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Help.Guides;

public class HelpGuideAddingSongsBuilder : DiscordComponentBuilder
{
    public IMessageComponentProperties?[] Build() {
        var container = new ComponentContainerProperties();

        container.AddComponents(
            MediaGalleryProperties.Create([
                new MediaGalleryItemProperties(new ComponentMediaProperties("https://storage.marsoau.com/published/readme/images/copy-token.png?cache"))
            ]),
            new ComponentSeparatorProperties(),
            new TextDisplayProperties(
                """
                ### `/add` `{songs}`: **Adds songs to your queue**
                > For songs you can use their ids, associations, urls, or PEQL points like favorite
                ### `/add-playlist` `{playlists}`: **Adds songs from playlists to your queue**
                > For playlists you can use their ids, names, and the favorite point too
                ## Some examples
                `/add` `https://www.youtube.com/watch?v=nnmIzzCLmrU`
                
                `/add` `5`
                
                `/add` `35,14,22`
                
                `/add` `some,associations`
                
                `/add` `https://www.youtube.com/watch?v=nnmIzzCLmrU,35,playlist(teto)`
                """
            )
        );
        
        return [
            container
        ];
    }
}
