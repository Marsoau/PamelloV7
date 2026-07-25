using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Help.Guides;

public class HelpGuideManagingQueueBuilder : DiscordComponentBuilder
{
    public IMessageComponentProperties?[] Build() {
        var container = new ComponentContainerProperties();

        container.AddComponents(
            MediaGalleryProperties.Create([
                new MediaGalleryItemProperties(new ComponentMediaProperties("https://storage.marsoau.com/published/readme/images/copy-token.png"))
            ]),
            new ComponentSeparatorProperties(),
            new TextDisplayProperties(
                """
                ### `/queue`: **Brings up a real-time interactive queue**
                Here you can see your selected player's queue songs, as well as some action buttons
                ## What the buttons do
                - `Add Songs`: Brings up a modal for you to query more songs, you can paste urls there or any other PEQL queries
                - `Edit`: Brings up a modal with which you can edit the queue by changing the songs order, removing songs, or adding new ones with PEQL (so ids, urls, points, etc.) into any position
                - `Go-To`: Brings up a modal to select a position to jump to right now, and it has an option to get back to the current song after the new one ends
                - `Set Next`: Brings up a modal to select a position that will be played after the current song ends
                
                > You can browse pages with the `Prev`, `Page`, `Next` buttons, the `Page` button in particular brings up a modal to select a specific page number
                """
            )
        );
        
        return [
            container
        ];
    }
}
