using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Help.Guides;

public class HelpGuideManagingUserBuilder : DiscordComponentBuilder
{
    public IMessageComponentProperties?[] Build() {
        var container = new ComponentContainerProperties();

        container.AddComponents(
            MediaGalleryProperties.Create([
                new MediaGalleryItemProperties(new ComponentMediaProperties("https://storage.marsoau.com/share/video/managing-user.mp4"))
            ]),
            new ComponentSeparatorProperties(),
            new TextDisplayProperties(
                """
                ### `/user info` `{user?}`: **Brings up real-time interactive user info**
                > Accepts a user as a parameter, when not specified uses your `current` user
                
                Here you can see the user join date (the first time the user interacted with Pamello), favorite songs and playlists count, and an authorizations preview
                ## What the "Show" buttons do
                - **For Favorite Songs**: Sends the same message as `/song favorite list` does
                - **For Favorite Playlists**: Sends the same message as `/playlist favorite list` does
                - **For Authorizations**: Sends the same message as `/user authorization list` does
                ### `/user authorization list` `{user?}`: **Brings up a real-time interactive user authorizations list**
                > Accepts a user as a parameter, when not specified uses your `current` user
                
                Here you can see all authorizations of the user, each with its action button, as well as some general action buttons
                ## Modes
                There are 2 modes this command can be in, which determine what each authorization action button does:
                - `Select`: Selects an authorization for the user
                - `Delete`: Deletes the authorization from the user
                > The buttons to switch modes are located below the authorizations list itself
                ## Other action buttons
                - `Add Authorization`: Creates a new authorization and adds it to your user
                
                By adding an authorization to your user, you give it the right to be recognized as your user
                
                By adding multiple Discord authorizations you can bind multiple Discord accounts to one Pamello user, and have all of your data synchronized between them, because they will be recognized as the same user
                
                > You can browse pages with the `Prev`, `Page`, `Next` buttons, the `Page` button in particular brings up a modal to select a specific page number
                """
            )
        );
        
        return [
            container
        ];
    }
}