using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Help.Guides;

public class HelpGuideManagingPlaylistsBuilder : DiscordComponentBuilder
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
                ### `/playlist info` `{playlist}`: **Brings up real-time interactive playlist info**
                > Accepts a playlist as a required parameter

                Here you can see the playlist id, name, addition date, owner user, protection state, and all of its songs, as well as some action buttons
                ## What the buttons do
                - `Add to queue`: Adds this playlist's songs to your current queue, not available if you don't have one
                - `Rename`: Brings up a modal for you to input the new name
                - `Edit`: Brings up a modal with which you can edit the playlist by changing the songs order, removing songs, or adding new ones with PEQL (so ids, urls, points, etc.) into any position
                
                > Playlists have only one owner, but can be browsed / used by everyone. You can also protect your playlist from changes by other users, or leave it public
                """
            )
        );
        
        return [
            container
        ];
    }
}
