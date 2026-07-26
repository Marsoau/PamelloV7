using NetCord.Rest;
using PamelloV7.Module.Marsoau.NetCord.Builders.Base;

namespace PamelloV7.Module.Marsoau.NetCord.Builders.Help.Guides;

public class HelpGuideManagingPlayerBuilder : DiscordComponentBuilder
{
    public IMessageComponentProperties?[] Build() {
        var container = new ComponentContainerProperties();

        container.AddComponents(
            MediaGalleryProperties.Create([
                new MediaGalleryItemProperties(new ComponentMediaProperties("https://github.com/user-attachments/assets/db21276e-5c6f-4bf7-9010-757bf6bb093f"))
            ]),
            new TextDisplayProperties(
                """
                ### `/player info`: **Brings up real-time interactive player info**
                """
            ),
            new ComponentSeparatorProperties(),
            new TextDisplayProperties(
                """
                
                Here you can see your selected player's playback time, current song, episode, queue modes, and some action buttons
                
                Also at the bottom you can see the currently connected speakers, and who is listening to them
                
                > If you don't have a selected player, you can create one with a button inside of the message, or choose an available one the same way
                ## What the buttons do
                - `Pause`: Pauses/Resumes the playback
                - `Next Episode`: Skips to the next episode in a song, only appears if the song has episodes
                - `Add Songs`: Brings up a modal for you to query more songs, you can paste urls there or any other PEQL queries
                - `Rewind`: Brings up a modal in which you can enter the time you want to rewind the currently playing song to
                - `Skip`: Just skips the current song
                ## What the queue modes do
                - `Random`: Plays queue songs in a random order
                - `Reversed`: Plays queue songs in a reversed order
                - `No Leftovers`: Removes songs from the queue after they are played
                - `Feed Random`: Feeds the queue with random songs from the database when it gets empty
                ### `/player pause-toggle`: **Toggles the pause state of your player**
                > Responds with an interactive message to switch it back when you want
                """
            )
        );
        
        return [
            container
        ];
    }
}