using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Help.Guides;

public class HelpGuideManagingSongsBuilder : DiscordComponentBuilder
{
    public IMessageComponentProperties?[] Build() {
        var container = new ComponentContainerProperties();

        container.AddComponents(
            MediaGalleryProperties.Create([
                new MediaGalleryItemProperties(new ComponentMediaProperties("https://github.com/user-attachments/assets/5dc078f1-b519-4e29-b744-19fc79200f39"))
            ]),
            new TextDisplayProperties(
                """
                ### `/song info` `{song?}`: **Brings up real-time interactive song info**
                > Accepts a song as a parameter, when not specified uses your `current` song
                """
            ),
            new ComponentSeparatorProperties(),
            new TextDisplayProperties(
                """
                
                Here you can see the song name, cover, id, addition date, adder user, some action buttons and dedicated sections
                ## What the buttons do
                - `Rename`: Brings up a modal for you to input the new name
                - `Change Cover`: Not implemented yet
                - `Reset`: Resets the song basic info like name, cover, and episodes to its source
                - `Add to queue`: Adds this song to your current queue, not available if you don't have one
                ## About the sections
                **Associations**: A list of associations for that song
                - `Edit`: Brings up a modal for you to change/add/remove associations
                
                **Favorite By Users**: A list of users that added this song to their favorites
                - `Add`: Adds this song to your favorites
                
                **Included In Playlists**: A list of playlists this song is included in
                - `Remove All`: Removes this song from all playlists
                
                **Sources**: A list of sources for that song
                - `Select`: Brings up a modal to select the main source for that song, this source will be used when resetting its info
                
                **Episodes**: Displays the count of episodes that song has, hidden if the song doesn't have any
                - `Show`: Sends the same message as the `/song episode list` command
                """
            )
        );
        
        return [
            container
        ];
    }
}