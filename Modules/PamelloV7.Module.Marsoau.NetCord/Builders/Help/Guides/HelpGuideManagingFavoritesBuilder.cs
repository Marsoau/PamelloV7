using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Help.Guides;

public class HelpGuideManagingFavoritesBuilder : DiscordComponentBuilder
{
    public IMessageComponentProperties?[] Build() {
        var container = new ComponentContainerProperties();

        container.AddComponents(
            MediaGalleryProperties.Create([
                new MediaGalleryItemProperties(new ComponentMediaProperties("https://github.com/user-attachments/assets/df6add2a-431a-4246-bdbc-eebb0a5b9d35"))
            ]),
            new TextDisplayProperties(
                """
                ### `/song favorite list` `{user?}`: **Brings up a real-time interactive favorite songs list**
                > Accepts a user as a parameter, when not specified uses your `current` user
                """
            ),
            new ComponentSeparatorProperties(),
            new TextDisplayProperties(
                """

                Here you can see all of your or another user's favorite songs, as well as some action buttons
                ## What the buttons do
                - `Edit`: Edits your favorite songs, so you can move/remove/add songs with PEQL (so ids, urls, points, etc.). Not available if these are not your favorites
                - `Clear`: Clears your favorite songs. Not available if these are not your favorites
                - `Add all to queue`: Adds all of your favorite songs to your current queue, not available if you don't have one
                > You can browse pages with the `Prev`, `Page`, `Next` buttons, the `Page` button in particular brings up a modal to select a specific page number
                ### `/playlist favorite list` `{user?}`: **Brings up a real-time interactive favorite playlists list**
                > Accepts a user as a parameter, when not specified uses your `current` user
                """
            )
        );
        
        return [
            container
        ];
    }
}
