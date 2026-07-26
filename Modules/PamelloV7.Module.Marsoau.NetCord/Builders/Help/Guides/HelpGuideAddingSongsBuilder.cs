using NetCord;
using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Help.Guides;

public class HelpGuideAddingSongsBuilder : DiscordComponentBuilder
{
    public IMessageComponentProperties?[] Build() {
        var container = new ComponentContainerProperties();

        container.AddComponents(
            MediaGalleryProperties.Create([
                new MediaGalleryItemProperties(new ComponentMediaProperties("https://github.com/user-attachments/assets/f2057a7c-1467-4038-a758-79acf260e7bc"))
            ]),
            new TextDisplayProperties(
                """
                ### `/add` `{songs}`: **Adds songs to your queue**
                > For songs you can use their ids, associations, urls, or PEQL points like favorite
                """
            ),
            new ComponentSeparatorProperties(),
            MediaGalleryProperties.Create([
                new MediaGalleryItemProperties(new ComponentMediaProperties("https://github.com/user-attachments/assets/a8c9acdf-635e-48a0-a3d6-6f2d85da3e7e"))
            ]),
            new TextDisplayProperties(
                """
                ### `/add-playlist` `{playlists}`: **Adds songs from playlists to your queue**
                > For playlists you can use their ids, names, and the favorite point too
                """
            ),
            new ComponentSeparatorProperties(),
            new TextDisplayProperties(
                """
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
